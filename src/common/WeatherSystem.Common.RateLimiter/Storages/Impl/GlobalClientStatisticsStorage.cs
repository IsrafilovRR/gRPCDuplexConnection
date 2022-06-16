using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages.Impl;

/// <inheritdoc />
public class GlobalClientStatisticsStorage : IGlobalClientStatisticsStorage
{
    private readonly ConcurrentDictionary<string, ClientStatistics> _clientStatistics = new();

    /// <inheritdoc />
    public bool GetClientStatistics(string ipAddress, [MaybeNullWhen(false)] out ClientStatistics clientStatistics)
    {
        return _clientStatistics.TryGetValue(ipAddress, out clientStatistics);
    }

    /// <inheritdoc />
    public bool AddClientStatistics(string ipAddress, ClientStatistics clientStatistics)
    {
        return _clientStatistics.TryAdd(ipAddress, clientStatistics);
    }
}