using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages.Impl;

/// <inheritdoc />
public class ClientIndividualRequestLimitsCache : IClientIndividualRequestLimitsCache
{
    // pair = ip address -> limits
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private static TimeSpan _cacheItemExpirationTimeTimeSpan = TimeSpan.FromHours(1);

    /// <inheritdoc />
    public bool GetRequestLimitsByIpAddress(string ipAddress, [MaybeNullWhen(false)] out RequestLimits requestLimits)
    {
        return _cache.TryGetValue(ipAddress, out requestLimits);
    }

    /// <inheritdoc />
    public void AddOrUpdateRequestLimits(string ipAddress, RequestLimits requestLimits)
    {
        if (_cache.TryGetValue(ipAddress, out var _))
        {
            return;
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(ipAddress, requestLimits, cacheEntryOptions);
    }
}