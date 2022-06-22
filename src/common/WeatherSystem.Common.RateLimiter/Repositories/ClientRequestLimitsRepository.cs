using WeatherSystem.Common.RateLimiter.Models;

namespace WeatherSystem.Common.RateLimiter.Repositories;

/// <inheritdoc />
public class ClientRequestLimitsRepository : IClientRequestLimitsRepository
{
    /// <inheritdoc />
    public async IAsyncEnumerable<ClientRequestLimits> GetClientsRequestLimitsAsyncEnumerable()
    {
        // todo: in real code here should be connection to DB or another resource
        yield return new ClientRequestLimits("77.88.55.70", new RequestLimits
        {
            MaxRequests = 50,
            TimeWindow = TimeSpan.FromMinutes(30)
        });

        yield return new ClientRequestLimits("77.88.55.85", new RequestLimits
        {
            MaxRequests = 150,
            TimeWindow = TimeSpan.FromMinutes(10)
        });

        yield return new ClientRequestLimits("77.88.105.70", new RequestLimits
        {
            MaxRequests = 10,
            TimeWindow = TimeSpan.FromHours(1)
        });
    }
}