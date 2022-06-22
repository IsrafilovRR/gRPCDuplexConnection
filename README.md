# WeatherSystem.Common.RateLimiter

## Types of request limiters

You can specify or use several types of request limiter in time window:
- Use client individual repository and cache.
- Use attribute RequestLimitsAttribute on an endpoint
  (see the example: EventClient.Controllers.AggregationController )
- Use global settings, which is configured in Config file

### Individual request limits

We have the cache of individual request limits, which is updated periodically by ClientLimitsCacheUpdaterHostedService/
Hosted service take data from repository.

TODO: Repository is stubbed, we have to implement real DB.

## Priority of the limits

What is priority of that limits:
- Highest priority is set for individual client request limits from Cache or Repository.
- Second priority is set for separate endpoints
- And the last one - is global from config.

## How it works

All requests go through RequestLimiterMiddleware. At first step we should take a request limits.

A client, which has individual limits, does the request to the endpoint, that has its 
own request limits too. Here we take individual limits, because it is more priority. 

Is we don't have any special limits, like individual or endpoint, then we take global limits.

Then we send taken limit, ip address and endpoint (if it has own limits) to  ILimitsRequestCheckerService.