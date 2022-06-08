using Microsoft.AspNetCore.Mvc;
using WeatherSystem.EventClient.Storages;
using WeatherSystem.EventClient.Storages.Interfaces;

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

    [HttpPut]
    public async Task<ActionResult> SubscribeOnSensors([FromQuery] List<long> sensorIds)
    {
        await _subscriptionsStorage.AddRange(sensorIds);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> UnsubscribeOfSensors([FromQuery] List<long> sensorIds)
    {
        await _subscriptionsStorage.RemoveRange(sensorIds);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult> GetSubscriptions()
    {
        return Ok(_subscriptionsStorage.GetSubscriptions());
    }
}