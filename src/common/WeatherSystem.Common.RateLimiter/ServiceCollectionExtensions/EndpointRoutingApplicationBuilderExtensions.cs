using Microsoft.AspNetCore.Builder;
using WeatherSystem.Common.RateLimiter.Middlewares;

namespace WeatherSystem.Common.RateLimiter.ServiceCollectionExtensions;

public static class EndpointRoutingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestLimiter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLimiterMiddleware>();
    }
}