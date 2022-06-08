namespace WeatherSystem.EventClient.Storages;

public interface ISubscriptionsStorage
{
    delegate Task SubscriptionsHandler();

    event SubscriptionsHandler? Notify;

    IEnumerable<long> GetSubscriptions();
    Task AddRange(IEnumerable<long> sensorsIds);
    Task RemoveRange(IEnumerable<long> sensorsIds);
}