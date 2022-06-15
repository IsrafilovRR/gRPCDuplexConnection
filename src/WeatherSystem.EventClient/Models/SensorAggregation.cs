namespace WeatherSystem.EventClient.Models;

public class SensorAggregation
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
    public int MinCo2 { get; set; }
    public int MaxCo2 { get; set; }
    public DateTime CreatedAt { get; set; }
}