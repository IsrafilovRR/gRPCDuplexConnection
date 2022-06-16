namespace WeatherSystem.Common.RateLimiter.Models;

/// <summary>
/// Client statistics
/// </summary>
public class ClientStatistics
{
    /// <summary>
    /// First successful request datetime 
    /// </summary>
    public DateTime FirstRequestInFrameStartTime { get; set; }

    /// <summary>
    /// Request count
    /// </summary>
    public int RequestCount { get; set; }
}