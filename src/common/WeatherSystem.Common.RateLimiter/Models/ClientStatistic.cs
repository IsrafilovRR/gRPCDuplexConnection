namespace WeatherSystem.Common.RateLimiter.Models;

/// <summary>
/// 
/// </summary>
public class ClientStatistics
{
    /// <summary>
    /// 
    /// </summary>
    public DateTime FirstRequestInFrameStartTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int RequestCount { get; set; }
}