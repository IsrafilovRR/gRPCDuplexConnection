using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages;

/// <summary>
/// Aggregated states storage
/// </summary>
public interface ISensorStatesAggregatedStorage
{
    /// <summary>
    /// Add aggregation
    /// </summary>
    void AddAggregation(long sensorId, SensorEvent aggregatedEvent);

    /// <summary>
    /// Get last aggregation for each sensor
    /// </summary>
    /// <returns></returns>
    Dictionary<long, SensorEvent> GetLastAggregation();
}