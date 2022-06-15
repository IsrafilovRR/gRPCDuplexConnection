using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages;

/// <summary>
/// Sensor storage
/// </summary>
public interface ISensorStorage
{
    /// <summary>
    /// Add or update sensor
    /// </summary>
    /// <param name="sensorId">sensor id</param>
    /// <param name="sensor">sensor</param>
    void AddOrUpdateSensor(long sensorId, Sensor sensor);
    
    /// <summary>
    /// Get all available sensors
    /// </summary>
    IEnumerable<Sensor> GetAllSensors();
}