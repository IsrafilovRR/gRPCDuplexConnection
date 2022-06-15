using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.Controllers;

/// <summary>
/// Controller for getting sensor states 
/// </summary>
[Route("sensors")]
public class SensorController : ControllerBase
{
    private readonly ISensorStorage _sensorStorage;
    private readonly ISensorStatesStorage _sensorStatesStorage;

    public SensorController(ISensorStatesStorage sensorStatesStorage, ISensorStorage sensorStorage)
    {
        _sensorStatesStorage = sensorStatesStorage;
        _sensorStorage = sensorStorage;
    }

    /// <summary>
    /// Get one sensor state by id
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult> GetSensorState(int id)
    {
        if (_sensorStatesStorage.TryGetState(id, out var state))
        {
            return Ok(state);
        }

        return NotFound();
    }

    /// <summary>
    /// Get all available sensors state 
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetSensorsState()
    {
        return Ok(_sensorStatesStorage.GetAllSensorsWithStates());
    }
}