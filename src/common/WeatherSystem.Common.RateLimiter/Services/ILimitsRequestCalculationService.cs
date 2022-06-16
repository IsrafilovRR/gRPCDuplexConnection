using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Services;

/// <summary>
/// 
/// </summary>
public interface ILimitsRequestCalculationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="requestLimitsAttribute"></param>
    /// <returns></returns>
    bool IsGlobalRequestNumberExceeded(string ipAddress, RequestLimits requestLimitsAttribute);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="endpoint"></param>
    /// <param name="requestLimitsAttribute"></param>
    /// <returns></returns>
    bool IsSpecialEndpointRequestNumberExceeded(string ipAddress, string endpoint, RequestLimits requestLimitsAttribute);
}