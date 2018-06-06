using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ClientRateLimiter.Tests
{
    public class CallTrackerTests
    {
        [Fact]
        public void TrimCallsForRateLimits_ForNoRateLimits_KeepsAllCalls()
        {
            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(200);
            callTracker.TrimCallsForRateLimits();

            Assert.Single(callTracker.CallHistory);
        }

        [Fact]
        public void TrimCallsForRateLimits_ForOneRateLimit_TrimsToRateLimitAmount()
        {
            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(0);
            callTracker.CallWillHappenIn(100);
            callTracker.CallWillHappenIn(200);

            var rateLimit = new StandardRateLimit(2, TimeSpan.FromSeconds(1));

            callTracker.TrimCallsForRateLimits(rateLimit);

            Assert.Equal(2, callTracker.CallHistory.Count());
        }

        [Fact]
        public void TrimCallsForRateLimits_ForOneRateLimit_KeepsTheMostRecentCalls()
        {
            var now = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(now);

            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(0);
            callTracker.CallWillHappenIn(1000);

            var rateLimit = new StandardRateLimit(1, TimeSpan.FromSeconds(1));

            callTracker.TrimCallsForRateLimits(rateLimit);

            Assert.Single(callTracker.CallHistory);
            Assert.Equal(now.AddMilliseconds(1000), callTracker.CallHistory.Single());
        }

        [Fact]
        public void TrimCallsForRateLimits_ForTwoRateLimits_TrimsToMaximumRateLimitAmount()
        {
            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(0);
            callTracker.CallWillHappenIn(100);
            callTracker.CallWillHappenIn(200);

            var firstRateLimit = new StandardRateLimit(1, TimeSpan.FromSeconds(1));
            var secondRateLimit = new StandardRateLimit(2, TimeSpan.FromSeconds(1));

            callTracker.TrimCallsForRateLimits(firstRateLimit, secondRateLimit);

            Assert.Equal(2, callTracker.CallHistory.Count());
        }
    }
}
