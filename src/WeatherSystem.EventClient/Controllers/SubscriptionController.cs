using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventClient.Storages;

namespace WeatherSystem.EventClient.Controllers;

/// <summary>
/// Controller for subscriptions on sensors
/// </summary>
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionsStorage _subscriptionsStorage;
    
    public SubscriptionController(ISubscriptionsStorage subscriptionsStorage)
    {
        _subscriptionsStorage = subscriptionsStorage;
    }

    /// <summary>
    /// Subscribe on sensors
    /// </summary>
    /// <param name="sensorIds">Sensors ids</param>
    [HttpPut]
    public async Task<ActionResult> SubscribeOnSensors([FromQuery] List<long> sensorIds)
    {
        await _subscriptionsStorage.AddRange(sensorIds);
        return Ok();
    }

    /// <summary>
    /// Unsubscribe on sensors
    /// </summary>
    /// <param name="sensorIds">Sensors ids</param>
    [HttpDelete]
    public async Task<ActionResult> UnsubscribeOfSensors([FromQuery] List<long> sensorIds)
    {
        await _subscriptionsStorage.RemoveRange(sensorIds);
        return Ok();
    }

    /// <summary>
    /// Get all sensors
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetSubscriptions()
    {
        return Ok(_subscriptionsStorage.GetSubscriptions());
    }
}