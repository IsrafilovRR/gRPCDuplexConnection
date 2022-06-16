namespace WeatherSystem.Common.RateLimiter.Models;

/// <summary>
/// Client requests limits
/// </summary>
public class ClientRequestLimits
{
    public ClientRequestLimits(string ipAddress, RequestLimits requestLimits)
    {
        IpAddress = ipAddress;
        RequestLimits = requestLimits;
    }

    /// <summary>
    /// Ip address
    /// </summary>
    public string IpAddress { get; init; }

    /// <summary>
    /// Request limits
    /// </summary>
    public RequestLimits RequestLimits { get; init; }
}