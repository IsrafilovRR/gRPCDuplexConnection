using WeatherSystem.EventsGenerator.Models;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.HostedServices;

/// <summary>
/// Hosted service for init work, for example this one generates sensors
/// </summary>
public class InitHostedService : IHostedService
{
    private readonly ISensorStorage _sensorStorage;
    private readonly ILogger<InitHostedService> _logger;

    public InitHostedService(ISensorStorage sensorStorage, ILogger<InitHostedService> logger)
    {
        _sensorStorage = sensorStorage;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sensorStorage.AddOrUpdateSensor(1, new Sensor
        {
            Id = 1,
            Name = "HomeSensor",
            Type = SensorType.Inside
        });
        
        _sensorStorage.AddOrUpdateSensor(2, new Sensor
        {
            Id = 2,
            Name = "HomeSensor2",
            Type = SensorType.Inside
        });

        _sensorStorage.AddOrUpdateSensor(3, new Sensor
        {
            Id = 3,
            Name = "OutsideSensor",
            Type = SensorType.Outside
        });
        
        _sensorStorage.AddOrUpdateSensor(4, new Sensor
        {
            Id = 4,
            Name = "OutsideSensor2",
            Type = SensorType.Outside
        });

        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}