using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

/// <summary>
/// Client individual request limits cache
/// Synchronize with actual values by <see cref="WeatherSystem.Common."/>
/// </summary>
public interface IClientIndividualRequestLimitsCache
{
    /// <summary>
    /// Get RequestLimits by ip address
    /// </summary>
    bool GetRequestLimitsByIpAddress(string ipAddress, [MaybeNullWhen(false)] out RequestLimits requestLimits);

    /// <summary>
    /// Add or update request limits
    /// </summary>
    void AddOrUpdateRequestLimits(string ipAddress, RequestLimits requestLimits);
}