namespace WeatherSystem.Common.RateLimiter.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RequestLimitsAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public uint TimeWindow { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Measure Measure { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public uint MaxRequests { get; init; }

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