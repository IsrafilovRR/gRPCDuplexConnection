using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherSystem.Common.RateLimiter.Attributes;
using WeatherSystem.Common.RateLimiter.Models;
using WeatherSystem.Common.RateLimiter.Options;
using WeatherSystem.Common.RateLimiter.Services;
using WeatherSystem.Common.RateLimiter.Storages;

namespace WeatherSystem.Common.RateLimiter.Middlewares;

/// <summary>
/// Middleware for controlling request limits
/// </summary>
public class RequestLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestLimiterOptions _options;
    private readonly ILogger<RequestLimiterMiddleware> _logger;

    public RequestLimiterMiddleware(RequestDelegate next, IOptions<RequestLimiterOptions> options,
        ILogger<RequestLimiterMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _options = options?.Value ?? throw new ArgumentNullException("RequestLimiterOptions");
    }

    public async Task InvokeAsync(HttpContext context, IClientIndividualLimitsStorage clientIndividualLimitsStorage,
        ILimitsRequestCalculationService limitsRequestCalculationService)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null || context.Connection.RemoteIpAddress == null)
        {
            _logger.LogWarning($"Endpoint object is null or ip address is null");
            await _next(context);
        }
        else
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            var endpointString = context.Request.Path;
            var endpointLimitsAttribute = endpoint?.Metadata.GetMetadata<RequestLimitsAttribute>();
            var individualClientLimitsExists =
                clientIndividualLimitsStorage.GetRequestLimitsByIpAddress(ipAddress, out var individualClientLimits);

            var requestLimitsExceeded = true;

            // here we check if we have special limits for the endpoint or not 
            if (endpointLimitsAttribute != null)
            {
                var endpointLimits = new RequestLimits
                {
                    MaxRequests = endpointLimitsAttribute.MaxRequests,
                    TimeWindow = endpointLimitsAttribute.GetTimeWindowTimeSpan()
                };
                
                _logger.LogDebug($"Endpoint {endpointString} has its own limits. Will check it.");

                // if yes, we have to check endpoint limits
                // moreover if we have individual limits, then we apply them for the endpoint for current ip
                requestLimitsExceeded = limitsRequestCalculationService.IsSpecialEndpointRequestNumberExceeded(
                    ipAddress,
                    endpointString,
                    individualClientLimitsExists && individualClientLimits != null
                        ? individualClientLimits
                        : endpointLimits);
            }
            else
            {
                _logger.LogDebug($"Endpoint {endpointString} doesn't have its own limits.");
                // if not, then we have to check global limits
                // moreover if we have individual limits, then we apply them for as global limits for current ip
                requestLimitsExceeded = limitsRequestCalculationService.IsGlobalRequestNumberExceeded(ipAddress,
                    individualClientLimitsExists && individualClientLimits != null
                        ? individualClientLimits
                        : new RequestLimits
                        {
                            MaxRequests = _options.MaxRequests,
                            TimeWindow = _options.TimeWindow
                        });
            }

            if (requestLimitsExceeded)
            {
                _logger.LogInformation($"Limits exceeded for ip address {ipAddress} for endpoint {endpointString}");
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await _next(context);
        }
    }
}