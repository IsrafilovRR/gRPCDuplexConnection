using Microsoft.Extensions.Options;
using WeatherSystem.EventClient.Options;
using WeatherSystem.EventClient.Services;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.HostedServices;

/// <summary>
/// Hosted service with timer which every period (<see cref="AggregationOptions.AggregationPeriodInMinutes"/>) will calculate aggregation and
/// store it in storage
/// </summary>
public class AggregateSensorStatesHostedService : IHostedService, IDisposable
{
    private readonly ILogger<AggregateSensorStatesHostedService> _logger;
    private readonly ISensorStatesStorage _sensorStatesStorage;
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly AggregationOptions _options;

    private Timer? _timer = null;

    public AggregateSensorStatesHostedService(ILogger<AggregateSensorStatesHostedService> logger,
        ISensorStatesStorage sensorStatesStorage, ISensorStatesAggregatedStorage aggregatedStorage,
        IOptions<AggregationOptions> options, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _sensorStatesStorage = sensorStatesStorage;
        _aggregatedStorage = aggregatedStorage;
        _serviceProvider = serviceProvider;
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
        using var scope = _serviceProvider.CreateScope();
        var aggregationCalculationService = scope.ServiceProvider.GetRequiredService<IAggregationCalculationService>();
        
        var dictionary =
            _sensorStatesStorage.GetStatesForPeriod(TimeSpan.FromMinutes(_options.AggregationPeriodInMinutes));
        var dateTimeNow = DateTime.UtcNow;
        var dateTimeNowWithoutSeconds = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day,
            dateTimeNow.Hour, dateTimeNow.Minute, 0, dateTimeNow.Kind);

        foreach (var pair in dictionary)
        {
            if (pair.Value == null || pair.Value.Count == 0) continue;

            var sensorAggregation = aggregationCalculationService.GetAggregatedStateBySensorEvents(pair.Value);
            sensorAggregation.CreatedAt = dateTimeNowWithoutSeconds;
            
            _aggregatedStorage.AddAggregation(pair.Key, sensorAggregation);
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