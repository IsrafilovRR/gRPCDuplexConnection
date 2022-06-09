using System.Collections.Concurrent;
using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages.Impl;

/// <inheritdoc />
public class SensorStatesStorage : ISensorStatesStorage
{
    /// <summary>
    /// Use linked list as value here to iterate easily from the end of the sensors state
    /// </summary>
    private readonly ConcurrentDictionary<long, LinkedList<SensorEvent>> _sensorsStates = new();

    /// <inheritdoc />
    public void AddState(long sensorId, SensorEvent @event)
    {
        if (_sensorsStates.ContainsKey(sensorId))
        {
            _sensorsStates[sensorId].AddLast(@event);
            return;
        }

        _sensorsStates.TryAdd(sensorId, new LinkedList<SensorEvent>(new[] { @event }));
    }

    /// <inheritdoc />
    public Dictionary<long, List<SensorEvent>?> GetStatesForPeriod(TimeSpan timeSpan)
    {
        var result = new Dictionary<long, List<SensorEvent>?>();

        foreach (var sensorKey in _sensorsStates.Keys)
        {
            result.Add(sensorKey, GetSensorEventsByLastCreatedTime(sensorKey, timeSpan));
        }

        return result;
    }

    /// <inheritdoc />
    public IEnumerable<SensorEvent> GetStatesBySensorId(long sensorId)
    {
        return _sensorsStates.ContainsKey(sensorId) ? _sensorsStates[sensorId] : new List<SensorEvent>();
    }

    /// <summary>
    /// Get sensor event from timespan and till now
    /// </summary>
    private List<SensorEvent>? GetSensorEventsByLastCreatedTime(long sensorId, TimeSpan timeSpan)
    {
        if (!_sensorsStates.ContainsKey(sensorId))
        {
            return null;
        }

        var linkedList = _sensorsStates[sensorId];
        var result = new List<SensorEvent>();

        var last = linkedList.Last;
        while (last != null && DateTime.UtcNow - last.Value.CreatedAt < timeSpan)
        {
            result.Add(last.Value);
            last = last.Previous;
        }

        return result;
    }
}