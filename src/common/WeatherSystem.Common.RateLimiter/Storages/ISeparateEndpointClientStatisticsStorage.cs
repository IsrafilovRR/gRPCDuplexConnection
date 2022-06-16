using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

public interface ISeparateEndpointClientStatisticsStorage
{
    bool GetClientStatistic(string ipAddress, string endpoint, [MaybeNullWhen(false)] out ClientStatistics clientStatistics);

    bool AddClientStatistic(string ipAddress, string endpoint, ClientStatistics clientStatistics);
}