using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventClient.Models;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.Controllers;

/// <summary>
/// Controller for getting aggregated states of the sensors
/// </summary>
[Route("diagnostic")]
public class DiagnosticController : ControllerBase
{
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;
    private readonly ISensorStatesStorage _statesStorage;

    public DiagnosticController(ISensorStatesAggregatedStorage aggregatedStorage, ISensorStatesStorage statesStorage)
    {
        _aggregatedStorage = aggregatedStorage;
        _statesStorage = statesStorage;
    }

    [HttpGet("{sensorId:int}")]
    public async Task<ActionResult> GetSensorInformationById(int sensorId)
    {
        var aggregations = _aggregatedStorage.GetAggregationsBySensorId(sensorId);
        var states = _statesStorage.GetStatesBySensorId(sensorId);

        return Ok(new DiagnosticModel
        {
            Aggregations = aggregations.ToList(),
            Events = states.ToList(),
            SensorId = sensorId
        });
    }
}