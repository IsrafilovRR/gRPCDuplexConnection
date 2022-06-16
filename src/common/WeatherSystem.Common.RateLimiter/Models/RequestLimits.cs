namespace WeatherSystem.Common.RateLimiter.Models;

/// <summary>
/// 
/// </summary>
public class RequestLimits
{
    /// <summary>
    /// 
    /// </summary>
    public TimeSpan TimeWindow { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public uint MaxRequests { get; init; }
}