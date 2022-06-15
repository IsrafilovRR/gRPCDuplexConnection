namespace WeatherSystem.EventClient.Models;

public class DiagnosticModel
{
    public long SensorId { get; init; }
    public List<SensorEvent> Events { get; init; }
    public List<SensorAggregation> Aggregations { get; init; }
}