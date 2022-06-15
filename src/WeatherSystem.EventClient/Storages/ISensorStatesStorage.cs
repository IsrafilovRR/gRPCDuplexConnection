using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages;

/// <summary>
/// Sensors states storage
/// </summary>
public interface ISensorStatesStorage
{
    /// <summary>
    /// Add state of the sensor
    /// </summary>
    void AddState(long sensorId, SensorEvent @event);

    /// <summary>
    /// Get sensors states for a last timespan period
    /// </summary>
    Dictionary<long, List<SensorEvent>?> GetStatesForPeriod(TimeSpan timeSpan);
    
    /// <summary>
    /// Get states by sensor id
    /// </summary>
    IEnumerable<SensorEvent> GetStatesBySensorId(long sensorId);
}