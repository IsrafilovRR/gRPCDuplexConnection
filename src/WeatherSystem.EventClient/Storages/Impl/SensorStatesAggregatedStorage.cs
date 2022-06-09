using System.Collections.Concurrent;
using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages.Impl;

/// <inheritdoc />
public class SensorStatesAggregatedStorage : ISensorStatesAggregatedStorage
{
    private readonly ConcurrentDictionary<long, List<SensorEvent>> _sensorsStates = new();

    /// <inheritdoc />
    public void AddAggregation(long sensorId, SensorEvent aggregatedEvent)
    {
        if (_sensorsStates.ContainsKey(sensorId))
        {
            _sensorsStates[sensorId].Add(aggregatedEvent);
            return;
        }

        _sensorsStates.TryAdd(sensorId, new List<SensorEvent>
        {
            aggregatedEvent
        });
    }
    
    /// <inheritdoc />
    public Dictionary<long, SensorEvent> GetLastAggregation()
    {
        return _sensorsStates.ToDictionary(pair => pair.Key, pair => pair.Value[^1]);
    }
}