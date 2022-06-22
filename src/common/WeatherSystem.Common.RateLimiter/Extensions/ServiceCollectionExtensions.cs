using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherSystem.Common.RateLimiter.HostedServices;
using WeatherSystem.Common.RateLimiter.Options;
using WeatherSystem.Common.RateLimiter.Repositories;
using WeatherSystem.Common.RateLimiter.Services;
using WeatherSystem.Common.RateLimiter.Storages;
using WeatherSystem.Common.RateLimiter.Storages.Impl;

namespace WeatherSystem.Common.RateLimiter.Extensions;

/// <summary>
/// IServiceCollection extensions for Request limiter 
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestLimiterServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStorages();
        services.AddServices();
        services.AddHostedServices();
        services.AddConfiguration(configuration);

        return services;
    }

    private static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services.AddSingleton<IClientIndividualRequestLimitsCache, ClientIndividualRequestLimitsCache>();
        services.AddSingleton<IGlobalClientStatisticsStorage, GlobalClientStatisticsStorage>();
        services.AddSingleton<ISeparateEndpointClientStatisticsStorage, SeparateEndpointClientStatisticsStorage>();
        services.AddSingleton<IClientIndividualRequestLimitsCache, ClientIndividualRequestLimitsCache>();
        
        services.AddScoped<IClientRequestLimitsRepository, ClientRequestLimitsRepository>();

        return services;
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<ClientLimitsCacheUpdaterHostedService>();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RequestLimiterOptions>(configuration.GetSection("RequestLimiterOptions"));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ILimitsRequestCheckerService, LimitsRequestCheckerService>();

        return services;
    }
}