using System.Collections.Concurrent;
using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages.Impl;

/// <inheritdoc />
public class SensorStorage : ISensorStorage
{
    private readonly ConcurrentDictionary<long, Sensor> _sensors = new();

    /// <inheritdoc />
    public void AddOrUpdateSensor(long sensorId, Sensor sensor)
    {
        _sensors.AddOrUpdate(sensorId, sensor, (_, _) => sensor);
    }

    /// <inheritdoc />
    public IEnumerable<Sensor> GetAllSensors()
    {
        return _sensors.Values.ToList();
    }
}