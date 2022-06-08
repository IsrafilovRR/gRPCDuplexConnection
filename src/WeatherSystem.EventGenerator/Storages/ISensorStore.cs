using System.Collections.Concurrent;
using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages;

public interface ISensorStore
{
    void AddOrUpdateSensor(long sensorId, Sensor sensor);
    IEnumerable<Sensor> GetAllSensors();
}

public class SensorStore : ISensorStore
{
    private readonly ConcurrentDictionary<long, Sensor> _sensors = new();

    public void AddOrUpdateSensor(long sensorId, Sensor sensor)
    {
        _sensors.AddOrUpdate(sensorId, sensor, (_, _) => sensor);
    }

    public IEnumerable<Sensor> GetAllSensors()
    {
        return _sensors.Values.ToList();
    }
}