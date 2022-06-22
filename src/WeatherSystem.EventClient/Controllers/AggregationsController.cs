using Microsoft.AspNetCore.Mvc;
using WeatherSystem.Common.RateLimiter.Attributes;
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

    public AggregationsController(ISensorStatesAggregatedStorage aggregatedStorage,
        IAggregationCalculationService aggregationCalculationService)
    {
        _aggregatedStorage = aggregatedStorage;
        _aggregationCalculationService = aggregationCalculationService;
    }

    /// <summary>
    /// Get last aggregation
    /// </summary>
    [HttpGet("last")]
    [RequestLimits(MaxRequests = 20, TimeWindow = 1, Measure = Measure.Minutes)]
    public async Task<ActionResult> GetLastAggregation()
    {
        return Ok(_aggregatedStorage.GetLastAggregation());
    }

    /// <summary>
    /// Get aggregation based on other aggregations
    /// /aggregations?startTime=2012-12-31T22:00:00.000Z
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAggregationFromStartTime([FromQuery] DateTime startTime)
    {
        return Ok(_aggregationCalculationService.GetAggregatedStatesByStartTime(startTime));
    }
}