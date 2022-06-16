namespace WeatherSystem.Common.RateLimiter.Options;

/// <summary>
/// Request limits options
/// </summary>
public class RequestLimiterOptions
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