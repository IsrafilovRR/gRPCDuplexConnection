using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages.Impl;

public class ClientIndividualLimitsStorage : IClientIndividualLimitsStorage
{
    // pair = ip address -> limits
    private readonly ConcurrentDictionary<string, RequestLimits> _limitRequest = new();

    public bool GetRequestLimitsByIpAddress(string ipAddress, [MaybeNullWhen(false)] out RequestLimits requestLimits)
    {
        return _limitRequest.TryGetValue(ipAddress, out requestLimits);
    }

    public bool AddRequestLimits(string ipAddress, RequestLimits requestLimits)
    {
        return _limitRequest.TryAdd(ipAddress, requestLimits);
    }
}