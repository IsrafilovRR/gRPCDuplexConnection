using WeatherSystem.EventsGenerator.Models;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.HostedServices;

/// <summary>
/// Background service which generates values on all available sensors
/// </summary>
public class SensorStatesBackgroundService : BackgroundService
{
    private readonly ISensorStorage _sensorStorage;
    private readonly ISensorStatesStorage _sensorStatesStorage;
    private readonly ILogger<InitHostedService> _logger;
    private readonly Random _random = new();

    public SensorStatesBackgroundService(ISensorStorage sensorStorage, ILogger<InitHostedService> logger,
        ISensorStatesStorage sensorStatesStorage)
    {
        _sensorStorage = sensorStorage;
        _logger = logger;
        _sensorStatesStorage = sensorStatesStorage;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sensors = _sensorStorage.GetAllSensors();

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var sensor in sensors)
            {
                var sensorEvent = new Models.SensorState()
                {
                    Humidity = _random.Next(20, 80),
                    // temperature inside usually about 25 degrees
                    Temperature = sensor.Type == SensorType.Inside ? _random.Next(23, 26) : _random.Next(-30, 30),
                    // co2 ppm outside usually 400 ppm (google)
                    Co2 = sensor.Type == SensorType.Inside ? _random.Next(500, 2000) : _random.Next(350, 500),
                };

                _sensorStatesStorage.AddOrUpdateState(sensor.Id, sensorEvent);
            }
            
            // it is delay like every sensor changes his value every 0.5 sec, it is not related to sending events
            await Task.Delay(500, stoppingToken);
        }
    }
}