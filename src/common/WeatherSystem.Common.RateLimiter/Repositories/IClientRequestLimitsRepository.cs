using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Repositories;

/// <summary>
/// Client RequestLimits Repository
/// </summary>
public interface IClientRequestLimitsRepository
{
    /// <summary>
    /// Get clients requestLimits
    /// </summary>
    IAsyncEnumerable<ClientRequestLimits> GetClientsRequestLimitsAsyncEnumerable();
}