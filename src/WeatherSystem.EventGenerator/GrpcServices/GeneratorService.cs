using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using WeatherSystem.EventsGenerator.Options;
using WeatherSystem.EventsGenerator.Proto;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.GrpcServices;

public class EventGeneratorService : EventGenerator.EventGeneratorBase
{
    private readonly ISensorStatesStore _sensorStateStore;
    private readonly ILogger<EventGeneratorService> _logger;
    private readonly GeneratorOptions _generatorOptions;

    public EventGeneratorService(ISensorStatesStore sensorStateStore, ISensorStore sensorStore,
        ILogger<EventGeneratorService> logger, IOptions<GeneratorOptions> generatorSettings)
    {
        _sensorStateStore = sensorStateStore;
        _logger = logger;
        _generatorOptions = generatorSettings.Value ?? throw new ArgumentException("Generator settings object is null");
    }

    public override async Task EventStream(IAsyncStreamReader<GetSensorEventsRequest> requestStream,
        IServerStreamWriter<GetSensorEventsResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            var subscribedSensorIds = new HashSet<long>();

            Task.Run(async () =>
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
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

    private IEnumerable<GetSensorEventsResponse> GenerateSensorsEvents(long[] sensorsIds)
    {
        foreach (var sensorId in sensorsIds)
        {
            if (!_sensorStateStore.TryGetState(sensorId, out var sensorState))
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