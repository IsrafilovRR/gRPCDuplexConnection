using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

/// <summary>
/// Storage for clients global statistics
/// </summary>
public interface IGlobalClientStatisticsStorage
{
    /// <summary>
    /// Get client statistics
    /// </summary>
    bool GetClientStatistics(string ipAddress, [MaybeNullWhen(false)] out ClientStatistics clientStatistics);

    /// <summary>
    /// Add client statistics 
    /// </summary>
    bool AddClientStatistics(string ipAddress, ClientStatistics clientStatistics);
}