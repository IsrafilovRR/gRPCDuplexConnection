using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventClient.Services;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.Controllers;

/// <summary>
/// Controller for getting aggregated states of the sensors
/// </summary>
[Route("aggregations")]
public class AggregationsController : ControllerBase
{
    private readonly ISensorStatesAggregatedStorage _aggregatedStorage;
    private readonly IAggregationCalculationService _aggregationCalculationService;

    public AggregationsController(ISensorStatesAggregatedStorage aggregatedStorage, IAggregationCalculationService aggregationCalculationService)
    {
        _aggregatedStorage = aggregatedStorage;
        _aggregationCalculationService = aggregationCalculationService;
    }
    
    /// <summary>
    /// Get last aggregation
    /// </summary>
    [HttpGet("last")]
    public async Task<ActionResult> GetLastAggregation()
    {
        return Ok(_aggregatedStorage.GetLastAggregation());
    }
    
    /// <summary>
    /// Get aggregation based on other aggregations
    /// </summary>
    public async Task<ActionResult> GetAggregationFromStartTime([FromQuery] DateTime startTime)
    {
        return Ok(_aggregationCalculationService.GetAggregatedStatesByStartTime(startTime));
    }
}