using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages.Impl;

/// <inheritdoc />
public class SensorStatesStorage : ISensorStatesStorage
{
    private readonly ISensorStorage _sensorStorage;
    private readonly ConcurrentDictionary<long, SensorState> _sensorEvents = new();

    public SensorStatesStorage(ISensorStorage sensorStorage)
    {
        _sensorStorage = sensorStorage;
    }

    /// <inheritdoc />
    public void AddOrUpdateState(long sensorId, SensorState state)
    {
        _sensorEvents.AddOrUpdate(sensorId, state, (_, _) => state);
    }

    /// <inheritdoc />
    public IEnumerable<Sensor> GetAllSensorsWithStates()
    {
        foreach (var sensor in _sensorStorage.GetAllSensors())
        {
            if (!TryGetState(sensor.Id, out var sensorState)) continue;
            sensor.State = sensorState;
            yield return sensor;
        }
    }

    /// <inheritdoc />
    public bool TryGetState(long sensorId, [MaybeNullWhen(false)] out SensorState state)
    {
        return _sensorEvents.TryGetValue(sensorId, out state);
    }
}