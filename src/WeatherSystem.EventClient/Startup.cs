using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Polly;
using WeatherSystem.EventClient.HostedServices;
using WeatherSystem.EventClient.Options;
using WeatherSystem.EventClient.Services;
using WeatherSystem.EventClient.Storages;
using WeatherSystem.EventClient.Storages.Impl;
using WeatherSystem.EventsGenerator.Proto;

namespace WeatherSystem.EventClient
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            
            // hosted services
            services.AddHostedService<SensorEventsBackgroundService>();
            services.AddHostedService<AggregateSensorStatesHostedService>();

            // storages
            services.AddSingleton<ISubscriptionsStorage, SubscriptionsStorage>();
            services.AddSingleton<ISensorStatesStorage, SensorStatesStorage>();
            services.AddSingleton<ISensorStatesAggregatedStorage, SensorStatesAggregatedStorage>();
            
            services.AddScoped<IAggregationCalculationService, AggregationCalculationService>();

            services.Configure<AggregationOptions>(_configuration.GetSection("AggregationOptions"));
            
            services.AddGrpcClient<EventGenerator.EventGeneratorClient>(
                options =>
                {
                    options.Address = new Uri("https://localhost:7235/");
                });
        }


        public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
        }
    }