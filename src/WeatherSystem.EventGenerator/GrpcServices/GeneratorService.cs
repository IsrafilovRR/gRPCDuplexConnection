using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using WeatherSystem.EventsGenerator.Options;
using WeatherSystem.EventsGenerator.Proto;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.GrpcServices;

/// <summary>
/// Proto service which generates event to stream, also receives event, because duplex method here
/// </summary>
public class EventGeneratorService : EventGenerator.EventGeneratorBase
{
    private readonly ISensorStatesStorage _sensorStateStorage;
    private readonly ILogger<EventGeneratorService> _logger;
    private readonly GeneratorOptions _generatorOptions;

    public EventGeneratorService(ISensorStatesStorage sensorStateStorage, ISensorStorage sensorStorage,
        ILogger<EventGeneratorService> logger, IOptions<GeneratorOptions> generatorSettings)
    {
        _sensorStateStorage = sensorStateStorage;
        _logger = logger;
        _generatorOptions = generatorSettings.Value ?? throw new ArgumentException("Generator settings object is null");
    }

    public override async Task EventStream(IAsyncStreamReader<GetSensorEventsRequest> requestStream,
        IServerStreamWriter<GetSensorEventsResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            // hashSet of the subscribed ids
            var subscribedSensorIds = new HashSet<long>();

            Task.Run(async () =>
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // if we received a message from request stream, then we need to update our ids collection
                    while (await requestStream.MoveNext(context.CancellationToken))
                    {
                        var sensorIds = requestStream.Current.SensorId.ToList();
                        subscribedSensorIds.Clear();
                        subscribedSensorIds.UnionWith(sensorIds);
                    }
                }
            });

            while (!context.CancellationToken.IsCancellationRequested)
            {
                foreach (var sensorEvent in GenerateSensorsEvents(subscribedSensorIds.ToArray()))
                {
                    await responseStream.WriteAsync(sensorEvent, context.CancellationToken);
                    _logger.LogDebug("Send response: {0}", JsonSerializer.Serialize(sensorEvent));
                }

                await Task.Delay(_generatorOptions.EventsGenerationPeriodInMilliseconds);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("A operation was canceled");
        }
    }

    /// <summary>
    /// Method takes state of the requested sensors
    /// </summary>
    /// <param name="sensorsIds">Requested sensor ids</param>
    private IEnumerable<GetSensorEventsResponse> GenerateSensorsEvents(long[] sensorsIds)
    {
        foreach (var sensorId in sensorsIds)
        {
            if (!_sensorStateStorage.TryGetState(sensorId, out var sensorState))
            {
                continue;
            }

            yield return new GetSensorEventsResponse
            {
                State = new SensorState
                {
                    Co2 = sensorState.Co2,
                    Humidity = sensorState.Humidity,
                    Temperature = sensorState.Temperature
                },
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                SensorId = sensorId
            };
        }
    }
}