using System.Text.Json;
using Grpc.Core;
using Polly;
using WeatherSystem.EventClient.Models;
using WeatherSystem.EventClient.Storages;
using WeatherSystem.EventsGenerator.Proto;

namespace WeatherSystem.EventClient.HostedServices;

/// <summary>
/// Background service to support duplex connection with grpc server
/// </summary>
public class SensorEventsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ISubscriptionsStorage _subscriptionsStorage;
    private readonly ILogger<SensorEventsBackgroundService> _logger;
    private readonly ISensorStatesStorage _sensorStatesStorage;

    private const int MaxRetries = int.MaxValue;

    /// <summary>
    /// Ctor
    /// </summary>
    public SensorEventsBackgroundService(IServiceProvider provider, ISubscriptionsStorage subscriptionsStorage,
        ILogger<SensorEventsBackgroundService> logger, ISensorStatesStorage sensorStatesStorage)
    {
        _provider = provider;
        _logger = logger;
        _sensorStatesStorage = sensorStatesStorage;
        _subscriptionsStorage = subscriptionsStorage;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // create retry policy for reconnect to server 
        var retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(retryCount: MaxRetries,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
                {
                    _logger.LogWarning($"Retrying in {sleepDuration}. {attemptNumber} / {MaxRetries}");
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await using var scope = _provider.CreateAsyncScope();
                var client = scope.ServiceProvider.GetRequiredService<EventGenerator.EventGeneratorClient>();
                using var eventResponseStream =
                    client.EventStream(Metadata.Empty, DateTime.MaxValue, stoppingToken);

                async Task SendSubscriptionTask()
                {
                    await eventResponseStream.RequestStream.WriteAsync
                    (new GetSensorEventsRequest { SensorId = { _subscriptionsStorage.GetSubscriptions() } },
                        stoppingToken);
                }

                // if our connection to the server was lost after we had some subscription
                await SendSubscriptionTask();

                // add event if our subscriptions has been changed
                _subscriptionsStorage.Notify += async () => await SendSubscriptionTask();

                while (await eventResponseStream.ResponseStream.MoveNext(stoppingToken))
                {
                    var responseStreamCurrent = eventResponseStream.ResponseStream.Current;
                    
                    _logger.LogDebug(JsonSerializer.Serialize(responseStreamCurrent));
                    
                    _sensorStatesStorage.AddState(responseStreamCurrent.SensorId, new Models.SensorEvent()
                    {
                        Co2 = responseStreamCurrent.State.Co2,
                        Humidity = responseStreamCurrent.State.Humidity,
                        Temperature = responseStreamCurrent.State.Temperature,
                        CreatedAt = responseStreamCurrent.CreatedAt.ToDateTime()
                    });
                }
            }
            catch (RpcException exception)
            {
                _logger.LogWarning(
                    $"Caught the rpc exception. Seems something with connection. Message: {exception.Message}",
                    exception);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
                throw;
            }
        });
    }
}