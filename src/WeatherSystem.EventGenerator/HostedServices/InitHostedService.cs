using WeatherSystem.EventsGenerator.Models;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.HostedServices;

public class InitHostedService : IHostedService
{
    private readonly ISensorStore _sensorStore;
    private readonly ILogger<InitHostedService> _logger;

    public InitHostedService(ISensorStore sensorStore, ILogger<InitHostedService> logger)
    {
        _sensorStore = sensorStore;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sensorStore.AddOrUpdateSensor(1, new Sensor
        {
            Id = 1,
            Name = "HomeSensor",
            Type = SensorType.Inside
        });

        _sensorStore.AddOrUpdateSensor(2, new Sensor
        {
            Id = 2,
            Name = "OutsideSensor",
            Type = SensorType.Outside
        });
        
        _sensorStore.AddOrUpdateSensor(2, new Sensor
        {
            Id = 3,
            Name = "OutsideSensor",
            Type = SensorType.Outside
        });
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}