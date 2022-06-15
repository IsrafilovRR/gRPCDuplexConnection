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
    void AddAggregation(long sensorId, SensorAggregation aggregatedEvent);

    /// <summary>
    /// Get last aggregation for each sensor
    /// </summary>
    Dictionary<long, SensorAggregation> GetLastAggregation();

    /// <summary>
    /// Get aggregations started from some start time until now
    /// </summary>
    /// <param name="startTime">Start time</param>
    Dictionary<long, List<SensorAggregation>> GetAggregationsStartedFromDateTime(DateTime startTime);
    
    /// <summary>
    /// Get aggregations by sensor id
    /// </summary>
    IEnumerable<SensorAggregation> GetAggregationsBySensorId(long sensorId);
}