namespace WeatherSystem.EventClient.Storages;

public class SubscriptionsStorage : ISubscriptionsStorage
{
    private readonly HashSet<long> _subscriptions = new();
    public event ISubscriptionsStorage.SubscriptionsHandler? Notify;

    public IEnumerable<long> GetSubscriptions()
    {
        return _subscriptions.ToArray();
    }

    public async Task AddRange(IEnumerable<long> sensorsIds)
    {
        _subscriptions.UnionWith(sensorsIds);
        if (Notify != null) await Notify.Invoke();
    }

    public async Task RemoveRange(IEnumerable<long> sensorsIds)
    {
        foreach (var id in sensorsIds)
        {
            _subscriptions.RemoveWhere(l => l == id);
        }
        if (Notify != null) await Notify.Invoke();
    }
}