namespace WeatherSystem.EventsGenerator.Models;

public class Sensor
{
    public long Id { get; set; }

    public string Name { get; set; }

    public SensorType Type { get; set; }

    public SensorState? State { get; set; }
}