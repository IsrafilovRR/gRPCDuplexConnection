using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Polly;
using WeatherSystem.EventClient.HostedServices;
using WeatherSystem.EventClient.Storages;
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
            services.AddHostedService<SensorEventsBackgroundService>();
            services.AddSingleton<ISubscriptionsStorage, SubscriptionsStorage>();

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