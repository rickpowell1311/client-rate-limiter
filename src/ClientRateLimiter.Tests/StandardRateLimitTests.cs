using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClientRateLimiter.Tests
{
    public class StandardRateLimitTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void NewStandardRateLimit_ShouldOnlyAllowPositiveAmount(int amount)
        {
            Assert.Throws<ArgumentException>(() => new StandardRateLimit(amount, TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void GetNextAllowedCallTime_WhenNoPreviousCallsHaveBeenTracked_ShouldReturnZero()
        {
            var callTracker = new CallTracker();
            var standardRateLimit = new StandardRateLimit(1, TimeSpan.FromSeconds(1));

            var nextAllowedCallTime = standardRateLimit.GetNextAllowedCallTime(callTracker);

            Assert.Equal(0, nextAllowedCallTime);
        }

        [Theory]
        [InlineData(3000)]
        [InlineData(5000)]
        public void GetNextAllowedCallTime_WhenOnePreviousCallTrackedForCurrentTime_ShouldBeNowPlusTimespanRange(int millisecondsTimespan)
        {
            var currentDate = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(currentDate);

            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(0);

            var standardRateLimit = new StandardRateLimit(1, TimeSpan.FromMilliseconds(millisecondsTimespan));

            var nextAllowedCallTime = standardRateLimit.GetNextAllowedCallTime(callTracker);

            Assert.Equal(millisecondsTimespan, nextAllowedCallTime);

            ReferenceTime.Unfreeze();
        }

        [Theory]
        [InlineData(5000)]
        public void GetNextAllowedCallTime_WhenOnePreviousCallWasMadeMoreThanTimespanAgo_ReturnsZero(int lastCallMillisecondsAgo)
        {
            var currentDate = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(currentDate);

            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(-lastCallMillisecondsAgo);

            var standardRateLimit = new StandardRateLimit(1, TimeSpan.FromMilliseconds(lastCallMillisecondsAgo - 100));

            var nextAllowedCallTime = standardRateLimit.GetNextAllowedCallTime(callTracker);

            Assert.Equal(0, nextAllowedCallTime);

            ReferenceTime.Unfreeze();
        }

        [Fact]
        public void GetNextAllowedCallTime_ForTwoPreviousCalls_ReturnsCorrectTimespan()
        {
            var currentDate = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(currentDate);

            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(10);
            callTracker.CallWillHappenIn(30);

            var standardRateLimit = new StandardRateLimit(2, TimeSpan.FromMilliseconds(100));
            var nextAllowedCallTime = standardRateLimit.GetNextAllowedCallTime(callTracker);

            // 2 calls allowed every 100 milliseconds, so next call should be allowed in 110 milliseconds 
            Assert.Equal(110, nextAllowedCallTime);

            ReferenceTime.Unfreeze();
        }
    }
}
