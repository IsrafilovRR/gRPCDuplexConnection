namespace WeatherSystem.Common.RateLimiter.Models;

/// <summary>
/// Request limits
/// </summary>
public class RequestLimits
{
    /// <summary>
    /// Time window
    /// </summary>
    public TimeSpan TimeWindow { get; init; }

    /// <summary>
    /// Max requests
    /// </summary>
    public uint MaxRequests { get; init; }
}