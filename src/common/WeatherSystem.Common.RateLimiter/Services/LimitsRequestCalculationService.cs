using Microsoft.Extensions.Logging;
using WeatherSystem.Common.RateLimiter.Models;
using WeatherSystem.Common.RateLimiter.Storages;

namespace WeatherSystem.Common.RateLimiter.Services;

public class LimitsRequestCalculationService : ILimitsRequestCalculationService
{
    private readonly IGlobalClientStatisticsStorage _globalClientStatisticsStorage;
    private readonly ISeparateEndpointClientStatisticsStorage _endpointClientStatisticsStorage;
    private readonly ILogger<LimitsRequestCalculationService> _logger;

    public LimitsRequestCalculationService(IGlobalClientStatisticsStorage globalClientStatisticsStorage,
        ISeparateEndpointClientStatisticsStorage endpointClientStatisticsStorage,
        ILogger<LimitsRequestCalculationService> logger)
    {
        _globalClientStatisticsStorage = globalClientStatisticsStorage;
        _endpointClientStatisticsStorage = endpointClientStatisticsStorage;
        _logger = logger;
    }

    public bool IsGlobalRequestNumberExceeded(string ipAddress, RequestLimits requestLimits)
    {
        var statisticSuccessfullyAdded = false;
        
        // firstly we have to check if statistics exists for the client with ip address
        if (!_globalClientStatisticsStorage.GetClientStatistic(ipAddress, out var globalStatistics))
        {
            statisticSuccessfullyAdded = _globalClientStatisticsStorage.AddClientStatistic(ipAddress,
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

        lock (globalStatistics)
        {
            // if predicate true that means we passed frame and should reset request count
            if (DateTime.UtcNow - globalStatistics.FirstRequestInFrameStartTime > requestLimits.TimeWindow)
            {
                globalStatistics.RequestCount = 1;
                globalStatistics.FirstRequestInFrameStartTime = DateTime.UtcNow;
                
                _logger.LogDebug($"Started new fixed window frame for ip address {ipAddress}.");
                return false;
            }

            globalStatistics.RequestCount++;
            
            _logger.LogDebug($"Request count for ip address {ipAddress} is {globalStatistics.RequestCount}.");
            
            return globalStatistics.RequestCount > requestLimits.MaxRequests;
        }
    }

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

        lock (endpointStatistics)
        {
            // if predicate true that means we passed frame and should reset request count
            if (DateTime.UtcNow - endpointStatistics.FirstRequestInFrameStartTime > requestLimits.TimeWindow)
            {
                endpointStatistics.RequestCount = 1;
                endpointStatistics.FirstRequestInFrameStartTime = DateTime.UtcNow;
                return false;
            }

            endpointStatistics.RequestCount++;
            return endpointStatistics.RequestCount > requestLimits.MaxRequests;
        }
    }
}