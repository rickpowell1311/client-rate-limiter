### Client Rate limiter ###

Configure a rate limiter, to consider one or more rate limits:

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
