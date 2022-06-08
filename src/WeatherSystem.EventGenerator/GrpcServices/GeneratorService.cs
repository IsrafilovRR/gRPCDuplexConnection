using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using WeatherSystem.EventsGenerator.Options;
using WeatherSystem.EventsGenerator.Proto;
using WeatherSystem.EventsGenerator.Storages;

namespace WeatherSystem.EventsGenerator.GrpcServices;

public class EventGeneratorService : EventGenerator.EventGeneratorBase
{
    private readonly ISensorStore _sensorStore;
    private readonly ISensorStatesStore _sensorStateStore;
    private readonly ILogger<EventGeneratorService> _logger;
    private readonly GeneratorSettings _generatorSettings;
    private readonly Random _random;

    public EventGeneratorService(ISensorStatesStore sensorStateStore, ISensorStore sensorStore,
        ILogger<EventGeneratorService> logger, IOptions<GeneratorSettings> generatorSettings)
    {
        _sensorStateStore = sensorStateStore;
        _sensorStore = sensorStore;
        _logger = logger;
        _generatorSettings = generatorSettings.Value ?? throw new ArgumentException("Generator settings is null");
        _random = new Random();
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
                        subscribedSensorIds.UnionWith(sensorIds);
                    }
                }
            });

            while (!context.CancellationToken.IsCancellationRequested)
            {
                foreach (var sensorEvent in GenerateSensorsEvents(subscribedSensorIds))
                {
                    await responseStream.WriteAsync(sensorEvent, context.CancellationToken);
                }

                await Task.Delay(_generatorSettings.EventsGenerationPeriodMs);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("A operation was canceled");
        }
    }

    private IEnumerable<GetSensorEventsResponse> GenerateSensorsEvents(HashSet<long> sensorsIds)
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