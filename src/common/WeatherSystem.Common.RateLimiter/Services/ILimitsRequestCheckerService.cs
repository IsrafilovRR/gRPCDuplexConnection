using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Services;

/// <summary>
/// LimitsRequests checker service
/// </summary>
public interface ILimitsRequestCheckerService
{
    /// <summary>
    /// Is global request number exceeded
    /// </summary>
    bool IsGlobalRequestNumberExceeded(string ipAddress, RequestLimits requestLimitsAttribute);
    
    /// <summary>
    /// Is special endpoint request number exceeded
    /// </summary>
    bool IsSpecialEndpointRequestNumberExceeded(string ipAddress, string endpoint, RequestLimits requestLimitsAttribute);
}