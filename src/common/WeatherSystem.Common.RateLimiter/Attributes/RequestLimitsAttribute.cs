namespace WeatherSystem.Common.RateLimiter.Attributes;

/// <summary>
/// Attribute to limit requests count during rime interval
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RequestLimitsAttribute : Attribute
{
    /// <summary>
    /// Max request
    /// </summary>
    public uint MaxRequests { get; init; }

    /// <summary>
    /// Time window 
    /// </summary>
    public uint TimeWindow { get; init; }

    /// <summary>
    /// Measure
    /// </summary>
    public Measure Measure { get; init; }

    public TimeSpan GetTimeWindowTimeSpan()
    {
        return this.Measure switch
        {
            Measure.Seconds => TimeSpan.FromSeconds(this.TimeWindow),
            Measure.Minutes => TimeSpan.FromMinutes(this.TimeWindow),
            Measure.Hours => TimeSpan.FromHours(this.TimeWindow),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum Measure
{
    Seconds,
    Minutes,
    Hours
}