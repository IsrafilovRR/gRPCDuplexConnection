using Microsoft.Extensions.Options;
using WeatherSystem.EventClient.Models;
using WeatherSystem.EventClient.Options;
using WeatherSystem.EventClient.Storages;
using WeatherSystem.EventClient.Storages.Interfaces;

namespace WeatherSystem.EventClient.HostedServices;

/// <summary>
///
/// </summary>
public class AggregateSensorStatesHostedService : IHostedService, IDisposable
{
    private readonly ILogger<AggregateSensorStatesHostedService> _logger;
    private readonly ISensorStatesStorage _sensorStatesStorage;
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;
    private readonly AggregationOptions _options;

    private Timer? _timer = null;

    public AggregateSensorStatesHostedService(ILogger<AggregateSensorStatesHostedService> logger,
        ISensorStatesStorage sensorStatesStorage, ISensorStatesAggregatedStorage aggregatedStorage, IOptions<AggregationOptions> options)
    {
        _logger = logger;
        _sensorStatesStorage = sensorStatesStorage;
        _aggregatedStorage = aggregatedStorage;
        _options = options?.Value ?? throw new ArgumentException("Aggregate options object is null");
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AggregateSensorStates Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(_options.AggregationPeriodInMinutes));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var dictionary = _sensorStatesStorage.GetStatesForPeriod(TimeSpan.FromMinutes(1));
        var dateTimeNow = DateTime.UtcNow;
        var dateTimeNowWithoutSeconds = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day,
            dateTimeNow.Hour, dateTimeNow.Minute, 0, dateTimeNow.Kind);

        foreach (var pair in dictionary)
        {
            var averageTemp = 0;
            var averageHumidity = 0;
            var averageCo2 = 0;
            
            if (pair.Value != null)
            {
                foreach (var sensorEvent in pair.Value)
                {
                    averageTemp += sensorEvent.Temperature;
                    averageHumidity += sensorEvent.Humidity;
                    averageCo2 += sensorEvent.Co2;
                }

                averageTemp /= pair.Value.Count;
                averageHumidity /= pair.Value.Count;
                averageCo2 /= pair.Value.Count;
            }

            _aggregatedStorage.AddAggregation(pair.Key, new SensorEvent
            {
                CreatedAt = dateTimeNowWithoutSeconds,
                Co2 = averageCo2,
                Temperature = averageTemp,
                Humidity = averageHumidity
            });
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AggregateSensorStates Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}