using Microsoft.AspNetCore.Builder;
using WeatherSystem.Common.RateLimiter.Middlewares;

namespace WeatherSystem.Common.RateLimiter.Extensions;

/// <summary>
/// IApplicationBuilder extensions for request limiter
/// </summary>
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestLimiter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLimiterMiddleware>();
        
    }
}