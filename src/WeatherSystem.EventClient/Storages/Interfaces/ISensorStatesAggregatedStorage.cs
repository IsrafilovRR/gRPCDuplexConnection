using WeatherSystem.EventClient.Models;

namespace WeatherSystem.EventClient.Storages.Interfaces;

public interface ISensorStatesAggregatedStorage
{
    void AddAggregation(long sensorId, SensorEvent aggregatedEvent);

    Dictionary<long, SensorEvent> GetLastAggregation();
}