using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClientRateLimiter.Tests
{
    public class RateLimiterTests
    {
        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimit_AddsCallToCallTracker()
        {
            var config = new RateLimiterConfiguration();
            config.AddRateLimit(new StandardRateLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            limiter.Limit(() => { });

            Assert.Single(limiter.CallHistory);
        }

        [Fact]
        public async Task RateLimiter_WhenOneCallMadeToLimitAsync_AddsCallToCallTracker()
        {
            var config = new RateLimiterConfiguration();
            config.AddRateLimit(new StandardRateLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            await limiter.LimitAsync(() => Task.CompletedTask);

            Assert.Single(limiter.CallHistory);
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimitWithReturn_AddsCallToCallTracker()
        {
            var config = new RateLimiterConfiguration();
            config.AddRateLimit(new StandardRateLimit(5, TimeSpan.FromSeconds(1)));

            Func<int> returningMethod = () => 5;

            var limiter = config.BuildRateLimiter();
            var result = limiter.Limit(returningMethod);

            Assert.Single(limiter.CallHistory);
        }

        [Fact]
        public async Task RateLimiter_WhenOneCallMadeToLimitAsyncWithReturn_AddsCallToCallTracker()
        {
            var config = new RateLimiterConfiguration();
            config.AddRateLimit(new StandardRateLimit(5, TimeSpan.FromSeconds(1)));

            Func<Task<int>> returningMethod = async () => await Task.FromResult(5);

            var limiter = config.BuildRateLimiter();
            var result = await limiter.Limit(returningMethod);

            Assert.Single(limiter.CallHistory);
        }

        [Fact]
        public async Task RateLimiter_WhenOneMoreCallThanLimitMadeInTimespan_LastCallTimeIsGreaterThanTimespanAfterFirstCall()
        {
            var callTimes = new List<DateTime>();

            Func<Task> callLogger = () =>
            {
                callTimes.Add(ReferenceTime.UtcNow);
                return Task.CompletedTask;
            };

            var rateLimitTimespan = TimeSpan.FromMilliseconds(500);

            var config = new RateLimiterConfiguration();
            config.AddRateLimit(new StandardRateLimit(3, rateLimitTimespan));

            var limiter = config.BuildRateLimiter();

            await Task.WhenAll(new List<Task>
            {
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger)
            });

            var firstCall = callTimes.Min();
            var lastCall = callTimes.Max();

            var elapsedTimeBetweenFirstAndLastCall = lastCall.Subtract(firstCall);

            Assert.True(
                elapsedTimeBetweenFirstAndLastCall.TotalMilliseconds > rateLimitTimespan.TotalMilliseconds, 
                $"Time between first and last call should be at least {rateLimitTimespan.TotalMilliseconds} milliseconds but was '{elapsedTimeBetweenFirstAndLastCall.TotalMilliseconds}' milliseconds");
        }
    }
}
