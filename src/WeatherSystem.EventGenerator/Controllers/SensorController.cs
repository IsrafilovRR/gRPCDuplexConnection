using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventsGenerator.Models;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.Controllers;

[Route("sensors")]
public class SensorController : ControllerBase
{
    private readonly ISensorStore _sensorStore;
    private readonly ISensorStatesStore _sensorStatesStore;

    public SensorController(ISensorStatesStore sensorStatesStore, ISensorStore sensorStore)
    {
        _sensorStatesStore = sensorStatesStore;
        _sensorStore = sensorStore;
    }

    [HttpGet("{id:long}")]
    public ActionResult GetSensorState(int id)
    {
        if (_sensorStatesStore.TryGetState(id, out var state))
        {
            return Ok(state);
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult GetSensorsState()
    {
        return Ok(_sensorStatesStore.GetAllSensorsWithStates());
    }
}