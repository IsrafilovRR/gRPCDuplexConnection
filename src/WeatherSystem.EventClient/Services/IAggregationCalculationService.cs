using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Services;

/// <summary>
/// Aggregation calculation service
/// </summary>
public interface IAggregationCalculationService
{
    /// <summary>
    /// Get aggregated states based on the scope of the aggregations for each sensor by start time
    /// </summary>
    Dictionary<long, SensorAggregation> GetAggregatedStatesByStartTime(DateTime startTime);

    /// <summary>
    /// Get aggregated state by sensor events
    /// </summary>
    SensorAggregation GetAggregatedStateBySensorEvents(List<SensorEvent> events);
}