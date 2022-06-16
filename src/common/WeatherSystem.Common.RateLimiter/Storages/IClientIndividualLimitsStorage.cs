using System.Diagnostics.CodeAnalysis;
using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Storages;

public interface IClientIndividualLimitsStorage
{
    bool GetRequestLimitsByIpAddress(string ipAddress, [MaybeNullWhen(false)] out RequestLimits requestLimits);

    bool AddRequestLimits(string ipAddress, RequestLimits requestLimits);
}