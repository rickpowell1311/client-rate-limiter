using System;
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

            Assert.Single(limiter.CallTimes);
        }
    }
}
