namespace WeatherSystem.Common.RateLimiter.Options;

/// <summary>
/// 
/// </summary>
public class RequestLimiterOptions
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