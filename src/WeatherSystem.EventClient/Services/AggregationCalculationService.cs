using WeatherSystem.EventClient.Models;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.Services;

/// <inheritdoc />
public class AggregationCalculationService : IAggregationCalculationService
{
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;

    public AggregationCalculationService(ISensorStatesAggregatedStorage aggregatedStorage)
    {
        _aggregatedStorage = aggregatedStorage;
    }

    /// <inheritdoc />
    public Dictionary<long, SensorAggregation> GetAggregatedStatesByStartTime(DateTime startTime)
    {
        var aggregations = _aggregatedStorage.GetAggregationsStartedFromDateTime(startTime);
        var result = new Dictionary<long, SensorAggregation>(aggregations.Keys.Count);

        foreach (var aggregation in aggregations
                     .Where(aggregation => aggregation.Value.Count != 0))
        {
            result.Add(aggregation.Key, CalculateAverageOfAggregations(aggregation.Value));
        }

        return result;
    }

    /// <inheritdoc />
    public SensorAggregation GetAggregatedStateBySensorEvents(List<SensorEvent> events)
    {
        var averageTemp = 0;
        var averageHumidity = 0;
        var minCo2 = 0;
        var maxCo2 = 0;

        foreach (var sensorEvent in events)
        {
            averageTemp += sensorEvent.Temperature;
            averageHumidity += sensorEvent.Humidity;
            minCo2 = minCo2 > sensorEvent.Co2 ? sensorEvent.Co2 : minCo2;
            maxCo2 = maxCo2 < sensorEvent.Co2 ? sensorEvent.Co2 : maxCo2;
        }

        averageTemp /= events.Count;
        averageHumidity /= events.Count;

        return new SensorAggregation
        {
            MinCo2 = minCo2,
            MaxCo2 = maxCo2,
            Temperature = averageTemp,
            Humidity = averageHumidity
        };
    }

    /// <summary>
    /// Get <see cref="SensorAggregation"/> average based on several aggregations 
    /// </summary>
    private static SensorAggregation CalculateAverageOfAggregations(List<SensorAggregation> aggregations)
    {
        var averageTemp = 0;
        var averageHumidity = 0;
        var minCo2 = 0;
        var maxCo2 = 0;

        foreach (var sensorAggregation in aggregations)
        {
            averageTemp += sensorAggregation.Temperature;
            averageHumidity += sensorAggregation.Humidity;
            minCo2 += minCo2 > sensorAggregation.MinCo2 ? sensorAggregation.MinCo2 : minCo2;
            maxCo2 += maxCo2 < sensorAggregation.MaxCo2 ? sensorAggregation.MaxCo2 : maxCo2;
        }

        averageTemp /= aggregations.Count;
        averageHumidity /= aggregations.Count;

        return new SensorAggregation
        {
            CreatedAt = DateTime.Now,
            MinCo2 = minCo2,
            MaxCo2 = maxCo2,
            Temperature = averageTemp,
            Humidity = averageHumidity
        };
    }
}