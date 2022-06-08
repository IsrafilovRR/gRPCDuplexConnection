using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages;

public interface ISensorStatesStore
{
    void AddOrUpdateState(long sensorId, SensorState state);

    IEnumerable<Sensor> GetAllSensorsWithStates();

    bool TryGetState(long sensorId, [MaybeNullWhen(false)] out SensorState state);
}

public class SensorStatesStore : ISensorStatesStore
{
    private readonly ISensorStore _sensorStore;
    private readonly ConcurrentDictionary<long, SensorState> _sensorEvents = new();

    public SensorStatesStore(ISensorStore sensorStore)
    {
        _sensorStore = sensorStore;
    }

    public void AddOrUpdateState(long sensorId, SensorState state)
    {
        _sensorEvents.AddOrUpdate(sensorId, state, (_, _) => state);
    }

    public IEnumerable<Sensor> GetAllSensorsWithStates()
    {
        foreach (var sensor in _sensorStore.GetAllSensors())
        {
            if (!TryGetState(sensor.Id, out var sensorState)) continue;
            sensor.State = sensorState;
            yield return sensor;
        }
    }

    public bool TryGetState(long sensorId, [MaybeNullWhen(false)] out SensorState state)
    {
        return _sensorEvents.TryGetValue(sensorId, out state);
    }
}