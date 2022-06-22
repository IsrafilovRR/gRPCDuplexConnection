using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

/// <summary>
/// Storage for client separate endpoint statistics
/// </summary>
public interface ISeparateEndpointClientStatisticsStorage
{
    /// <summary>
    /// Get client statistics
    /// </summary>
    bool GetClientStatistic(string ipAddress, string endpoint, [MaybeNullWhen(false)] out ClientStatistics clientStatistics);

    /// <summary>
    /// Add client statistics 
    /// </summary>
    bool AddClientStatistic(string ipAddress, string endpoint, ClientStatistics clientStatistics);
}