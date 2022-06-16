using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

public interface IGlobalClientStatisticsStorage
{
    bool GetClientStatistic(string ipAddress, [MaybeNullWhen(false)] out ClientStatistics clientStatistics);

    bool AddClientStatistic(string ipAddress, ClientStatistics clientStatistics);
}