using System.Collections.Concurrent;
using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages.Impl;

/// <inheritdoc />
public class SensorStatesAggregatedStorage : ISensorStatesAggregatedStorage
{
    private readonly ConcurrentDictionary<long, List<SensorAggregation>> _sensorsStates = new();

    /// <inheritdoc />
    public void AddAggregation(long sensorId, SensorAggregation aggregatedEvent)
    {
        if (_sensorsStates.ContainsKey(sensorId))
        {
            _sensorsStates[sensorId].Add(aggregatedEvent);
            return;
        }

        _sensorsStates.TryAdd(sensorId, new List<SensorAggregation>
        {
            aggregatedEvent
        });
    }

    /// <inheritdoc />
    public Dictionary<long, SensorAggregation> GetLastAggregation()
    {
        return _sensorsStates.ToDictionary(pair => pair.Key, pair => pair.Value[^1]);
    }

    /// <inheritdoc />
    public Dictionary<long, List<SensorAggregation>> GetAggregationsStartedFromDateTime(DateTime startTime)
    {
        var result = new Dictionary<long, List<SensorAggregation>>();

        foreach (var sensorsState in _sensorsStates)
        {
            var aggregatedEvents = sensorsState.Value
                .Where(aggregation => aggregation.CreatedAt > startTime)
                .ToList();

            if (aggregatedEvents.Count != 0)
            {
                result.Add(sensorsState.Key, aggregatedEvents);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public IEnumerable<SensorAggregation> GetAggregationsBySensorId(long sensorId)
    {
        return _sensorsStates.ContainsKey(sensorId) ? _sensorsStates[sensorId] : new List<SensorAggregation>();
    }
}