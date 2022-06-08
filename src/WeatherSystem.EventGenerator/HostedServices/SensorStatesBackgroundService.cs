using WeatherSystem.EventsGenerator.Models;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.HostedServices;

public class SensorStatesBackgroundService : BackgroundService
{
    private readonly ISensorStore _sensorStore;
    private readonly ISensorStatesStore _sensorStatesStore;
    private readonly ILogger<InitHostedService> _logger;
    private readonly Random _random = new();

    public SensorStatesBackgroundService(ISensorStore sensorStore, ILogger<InitHostedService> logger,
        ISensorStatesStore sensorStatesStore)
    {
        _sensorStore = sensorStore;
        _logger = logger;
        _sensorStatesStore = sensorStatesStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sensors = _sensorStore.GetAllSensors();

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

                _sensorStatesStore.AddOrUpdateState(sensor.Id, sensorEvent);
            }
            
            await Task.Delay(500, stoppingToken);
        }
    }
}