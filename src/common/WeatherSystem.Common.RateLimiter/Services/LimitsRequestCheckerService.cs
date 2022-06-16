using Microsoft.Extensions.Logging;
using WeatherSystem.Common.RateLimiter.Models;
using WeatherSystem.Common.RateLimiter.Storages;

namespace WeatherSystem.Common.RateLimiter.Services;

/// <inheritdoc />
public class LimitsRequestCheckerService : ILimitsRequestCheckerService
{
    private readonly IGlobalClientStatisticsStorage _globalClientStatisticsStorage;
    private readonly ISeparateEndpointClientStatisticsStorage _endpointClientStatisticsStorage;
    private readonly ILogger<LimitsRequestCheckerService> _logger;

    public LimitsRequestCheckerService(IGlobalClientStatisticsStorage globalClientStatisticsStorage,
        ISeparateEndpointClientStatisticsStorage endpointClientStatisticsStorage,
        ILogger<LimitsRequestCheckerService> logger)
    {
        _globalClientStatisticsStorage = globalClientStatisticsStorage;
        _endpointClientStatisticsStorage = endpointClientStatisticsStorage;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsGlobalRequestNumberExceeded(string ipAddress, RequestLimits requestLimits)
    {
        var statisticSuccessfullyAdded = false;

        // firstly we have to check if statistics exists for the client with ip address
        if (!_globalClientStatisticsStorage.GetClientStatistics(ipAddress, out var globalStatistics))
        {
            statisticSuccessfullyAdded = _globalClientStatisticsStorage.AddClientStatistics(ipAddress,
                new ClientStatistics
                {
                    RequestCount = 1,
                    FirstRequestInFrameStartTime = DateTime.UtcNow
                });
        }

        // means that we just created statistic successfully and then just can return false
        if (statisticSuccessfullyAdded)
        {
            _logger.LogDebug($"Created new client statistic for ip address: {ipAddress}");
            return false;
        }

        bool requestCountExceeded;
        lock (globalStatistics)
        {
            // if predicate true that means we passed frame and should reset request count
            if (DateTime.UtcNow - globalStatistics.FirstRequestInFrameStartTime > requestLimits.TimeWindow)
            {
                globalStatistics.RequestCount = 1;
                globalStatistics.FirstRequestInFrameStartTime = DateTime.UtcNow;
                requestCountExceeded = false;
            }
            else
            {
                globalStatistics.RequestCount++;
                requestCountExceeded = globalStatistics.RequestCount > requestLimits.MaxRequests;
            }
        }

        _logger.LogDebug($"Request count for {ipAddress} exceeded - {requestCountExceeded})");
        return requestCountExceeded;
    }

    /// <inheritdoc />
    public bool IsSpecialEndpointRequestNumberExceeded(string ipAddress, string endpoint, RequestLimits requestLimits)
    {
        var statisticSuccessfullyAdded = false;

        // firstly we have to check if statistics exists for the client with ip address
        if (!_endpointClientStatisticsStorage.GetClientStatistic(ipAddress, endpoint, out var endpointStatistics))
        {
            statisticSuccessfullyAdded = _endpointClientStatisticsStorage.AddClientStatistic(ipAddress, endpoint,
                new ClientStatistics
                {
                    RequestCount = 1,
                    FirstRequestInFrameStartTime = DateTime.UtcNow
                });
        }

        // means that we just created statistic successfully and then just can return false
        if (statisticSuccessfullyAdded)
        {
            return false;
        }

        bool requestCountExceeded;
        lock (endpointStatistics)
        {
            // if predicate true that means we passed frame and should reset request count
            if (DateTime.UtcNow - endpointStatistics.FirstRequestInFrameStartTime > requestLimits.TimeWindow)
            {
                endpointStatistics.RequestCount = 1;
                endpointStatistics.FirstRequestInFrameStartTime = DateTime.UtcNow;
                requestCountExceeded = false;
            }
            else
            {
                endpointStatistics.RequestCount++;
                requestCountExceeded = endpointStatistics.RequestCount > requestLimits.MaxRequests;
            }
        }

        _logger.LogDebug($"Request count for {ipAddress} and {endpoint} exceeded - {requestCountExceeded})");
        return requestCountExceeded;
    }
}