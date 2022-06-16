using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherSystem.Common.RateLimiter.Options;
using WeatherSystem.Common.RateLimiter.Services;
using WeatherSystem.Common.RateLimiter.Storages;
using WeatherSystem.Common.RateLimiter.Storages.Impl;

namespace WeatherSystem.Common.RateLimiter.ServiceCollectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestLimiterServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStorages();
        services.AddServices();
        services.Configure<RequestLimiterOptions>(configuration.GetSection("RequestLimiterOptions"));

        return services;
    }

    private static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services.AddSingleton<IClientIndividualLimitsStorage, ClientIndividualLimitsStorage>();
        services.AddSingleton<IGlobalClientStatisticsStorage, GlobalClientStatisticsStorage>();
        services.AddSingleton<ISeparateEndpointClientStatisticsStorage, SeparateEndpointClientStatisticsStorage>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ILimitsRequestCalculationService, LimitsRequestCalculationService>();

        return services;
    }
}