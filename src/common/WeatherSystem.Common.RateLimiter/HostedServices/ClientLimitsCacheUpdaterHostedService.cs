using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherSystem.Common.RateLimiter.Repositories;
using WeatherSystem.Common.RateLimiter.Storages;
using Timer = System.Timers.Timer;

namespace WeatherSystem.Common.RateLimiter.HostedServices;

/// <summary>
/// Hosted service with timer which updates the cache <see cref="IClientIndividualRequestLimitsCache"/>
/// </summary>
public class ClientLimitsCacheUpdaterHostedService : BackgroundService
{
    private readonly ILogger<ClientLimitsCacheUpdaterHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private const uint CacheUpdatePeriodMinutes = 5;

    private Timer? _timer = null;

    public ClientLimitsCacheUpdaterHostedService(IServiceProvider serviceProvider,
        ILogger<ClientLimitsCacheUpdaterHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var requestLimitsRepository =
                    scope.ServiceProvider.GetRequiredService<IClientRequestLimitsRepository>();
                var cache = scope.ServiceProvider.GetRequiredService<IClientIndividualRequestLimitsCache>();

                await foreach (var clientRequestLimits in
                               requestLimitsRepository.GetClientsRequestLimitsAsyncEnumerable()
                                   .WithCancellation(stoppingToken))
                {
                    cache.AddOrUpdateRequestLimits(clientRequestLimits.IpAddress, clientRequestLimits.RequestLimits);
                }

                _logger.LogInformation("Client request limits cache updated");
            }
            catch (Exception exception)
            {
                _logger.LogError("The exception handled during client request limits cache updating.", exception);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}