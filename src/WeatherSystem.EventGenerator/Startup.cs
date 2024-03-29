using WeatherSystem.EventsGenerator.GrpcServices;
using WeatherSystem.EventsGenerator.HostedServices;
using WeatherSystem.EventsGenerator.Options;
using WeatherSystem.EventsGenerator.Storages;
using WeatherSystem.EventsGenerator.Storages.Impl;

namespace WeatherSystem.EventsGenerator
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
            services.AddSingleton<ISensorStorage, SensorStorage>();
            services.AddSingleton<ISensorStatesStorage, SensorStatesStorage>();
            services.AddMvcCore();
            
            services.AddGrpc();

            services.AddHostedService<InitHostedService>();
            services.AddHostedService<SensorStatesBackgroundService>();

            services.Configure<GeneratorOptions>(_configuration.GetSection("GeneratorOptions"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<EventGeneratorService>();
            });
        }
    }
}