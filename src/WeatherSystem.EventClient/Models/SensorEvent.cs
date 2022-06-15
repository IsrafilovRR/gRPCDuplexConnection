namespace WeatherSystem.EventClient.Models;

public class SensorEvent
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
    public int Co2 { get; set; }
    public DateTime CreatedAt { get; set; }
}