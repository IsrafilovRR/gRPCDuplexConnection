using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages.Impl;

public class SeparateEndpointClientStatisticsStorage : ISeparateEndpointClientStatisticsStorage
{
    // key value pair is like that: ip address -> <endpoint, statistics> 
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ClientStatistics>> _clientStatistics =
        new();

    public bool GetClientStatistic(string ipAddress, string endpoint,
        [MaybeNullWhen(false)] out ClientStatistics clientStatistics)
    {
        if (_clientStatistics.TryGetValue(ipAddress, out var endpointStatisticsMap))
        {
            return endpointStatisticsMap.TryGetValue(endpoint, out clientStatistics);
        }

        ;
        clientStatistics = null;
        return false;
    }

    public bool AddClientStatistic(string ipAddress, string endpoint, ClientStatistics clientStatistics)
    {
        var endpointStatisticsMap = new ConcurrentDictionary<string, ClientStatistics>();
        endpointStatisticsMap.TryAdd(endpoint, clientStatistics);

        return _clientStatistics.TryAdd(ipAddress, endpointStatisticsMap);
    }
}