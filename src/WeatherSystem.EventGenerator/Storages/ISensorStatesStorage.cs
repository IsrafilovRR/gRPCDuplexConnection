using System.Diagnostics.CodeAnalysis;
using WeatherSystem.EventsGenerator.Models;

namespace WeatherSystem.EventsGenerator.Storages;

/// <summary>
/// Sensor state storage
/// </summary>
public interface ISensorStatesStorage
{
    /// <summary>
    /// Adds or updates the state  
    /// </summary>
    /// <param name="sensorId">Sensor id</param>
    /// <param name="state">State</param>
    void AddOrUpdateState(long sensorId, SensorState state);

    /// <summary>
    /// Get all available sensors
    /// </summary>
    IEnumerable<Sensor> GetAllSensorsWithStates();

    /// <summary>
    /// Try gets state by sensor id
    /// </summary>
    /// <param name="sensorId">Sensor id</param>
    /// <param name="state">State</param>
    bool TryGetState(long sensorId, [MaybeNullWhen(false)] out SensorState state);
}