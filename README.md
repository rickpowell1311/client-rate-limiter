### Client Rate Limiter ###

Client Rate Limiter is designed to restrict the number of calls that can be made to a method within a certain time frame

# Quick start

Configure a rate limiter to consider one or more rate limits:

```
var config = new RateLimiterConfiguration();
config.AddRateLimit(new StandardRateLimit(5, TimeSpan.FromSeconds(1)));
// Add more rate limits...

var limiter = config.BuildRateLimiter();
```

And then limit a call using the limiter API:

```
limiter.Limit(() => {});
// or...
await limiter.LimitAsync(() => {});
```

The Limit method and LimitAsync Task will not return until the rate limiter rules allow for it and the method has completed
