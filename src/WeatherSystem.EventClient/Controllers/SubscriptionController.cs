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

    [HttpPut]
    public ActionResult SubscribeOnSensors([FromQuery] List<long> sensorIds)
    {
        _subscriptionsStorage.AddRange(sensorIds);
        return Ok();
    }

    [HttpDelete]
    public ActionResult UnsubscribeOfSensors([FromQuery] List<long> sensorIds)
    {
        _subscriptionsStorage.RemoveRange(sensorIds);
        return Ok();
    }

    [HttpGet]
    public ActionResult GetSubscriptions()
    {
        return Ok(_subscriptionsStorage.GetSubscriptions());
    }
}