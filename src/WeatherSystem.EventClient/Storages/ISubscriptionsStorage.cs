namespace WeatherSystem.EventClient.Storages;

/// <summary>
/// Subscription storage
/// </summary>
public interface ISubscriptionsStorage
{
    /// <summary>
    /// Subscription handler delegate
    /// </summary>
    delegate Task SubscriptionsHandler();

    /// <summary>
    /// Notify event, when collection of the subscriptions has been changed
    /// </summary>
    event SubscriptionsHandler? Notify;

    /// <summary>
    /// Get all subscriptions 
    /// </summary>
    IEnumerable<long> GetSubscriptions();

    /// <summary>
    /// Add range subscriptions
    /// </summary>
    Task AddRange(IEnumerable<long> sensorsIds);
    
    /// <summary>
    /// Remove range subscriptions
    /// </summary>
    Task RemoveRange(IEnumerable<long> sensorsIds);
}