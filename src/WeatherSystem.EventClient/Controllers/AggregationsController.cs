using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.Controllers;

/// <summary>
/// Controller for getting aggregated states of the sensors
/// </summary>
[Route("aggregations")]
public class AggregationsController : ControllerBase
{
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;

    public AggregationsController(ISensorStatesAggregatedStorage aggregatedStorage)
    {
        _aggregatedStorage = aggregatedStorage;
    }
    
    [HttpGet("last")]
    public ActionResult GetLastAggregation()
    {
        return Ok(_aggregatedStorage.GetLastAggregation());
    }
}