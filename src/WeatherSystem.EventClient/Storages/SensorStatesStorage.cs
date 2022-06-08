using System.Collections.Concurrent;
using WeatherSystem.EventClient.Models;
using WeatherSystem.EventClient.Storages.Interfaces;

namespace WeatherSystem.EventClient.Storages;

public class SensorStatesStorage : ISensorStatesStorage
{
    private readonly ConcurrentDictionary<long, LinkedList<SensorEvent>> _sensorsStates = new();

    /// <inheritdoc />
    public void AddState(long sensorId, SensorEvent @event)
    {
        if (_sensorsStates.ContainsKey(sensorId))
        {
            _sensorsStates[sensorId].AddLast(@event);
            return;
        }
        _sensorsStates.TryAdd(sensorId, new LinkedList<SensorEvent>(new []{@event}));
    }

    public Dictionary<long, List<SensorEvent>?> GetStatesForPeriod(TimeSpan timeSpan)
    {
        var result = new Dictionary<long, List<SensorEvent>?>();
        
        foreach (var sensorKey in _sensorsStates.Keys)
        {
            result.Add(sensorKey, GetSensorEventsByLastCreatedTime(sensorKey, timeSpan));
        }

        return result;
    }

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