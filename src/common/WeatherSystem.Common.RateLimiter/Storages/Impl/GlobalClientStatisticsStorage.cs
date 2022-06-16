using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages.Impl;

public class GlobalClientStatisticsStorage : IGlobalClientStatisticsStorage
{
    private readonly ConcurrentDictionary<string, ClientStatistics> _clientStatistics = new();

    public bool GetClientStatistic(string ipAddress, [MaybeNullWhen(false)] out ClientStatistics clientStatistics)
    {
        return _clientStatistics.TryGetValue(ipAddress, out clientStatistics);
    }

    public bool AddClientStatistic(string ipAddress, ClientStatistics clientStatistics)
    {
        return _clientStatistics.TryAdd(ipAddress, clientStatistics);
    }
}