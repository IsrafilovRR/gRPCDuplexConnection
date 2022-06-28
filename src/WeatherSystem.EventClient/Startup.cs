using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Polly;
using WeatherSystem.Common.DataAccess;
using WeatherSystem.Common.DataAccess.Extensions;
using WeatherSystem.Common.RateLimiter.Extensions;
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

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // hosted services
            // services.AddHostedService<SensorEventsBackgroundService>();
            services.AddHostedService<AggregateSensorStatesHostedService>();

            // storages
            services.AddSingleton<ISubscriptionsStorage, SubscriptionsStorage>();
            services.AddSingleton<ISensorStatesStorage, SensorStatesStorage>();
            services.AddSingleton<ISensorStatesAggregatedStorage, SensorStatesAggregatedStorage>();

            services.AddRequestLimiterServices(_configuration);

            services.AddScoped<IAggregationCalculationService, AggregationCalculationService>();

            services.Configure<AggregationOptions>(_configuration.GetSection("AggregationOptions"));

            services.AddRepositories(_configuration);
            
            // services.AddGrpcClient<EventGenerator.EventGeneratorClient>(
            //     options => { options.Address = new Uri("https://localhost:7235/"); });

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseRequestLimiter();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}