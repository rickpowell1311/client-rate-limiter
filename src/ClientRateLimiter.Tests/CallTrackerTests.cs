using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ClientRateLimiter.Tests
{
    public class CallTrackerTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void TrimToLastXCallTimes_ForNegativeNumberOfCalls_ThrowsArgumentException(int numberOfCalls)
        {
            var callTracker = new CallTracker();

            Assert.Throws<ArgumentException>(() => callTracker.TrimToLastXCallTimes(numberOfCalls));
        }

        [Fact]
        public void TrimToLastZeroCallTimes_ForMoreThanZeroCalls_TrimsToEmptyList()
        {
            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(0);

            callTracker.TrimToLastXCallTimes(0);

            Assert.Empty(callTracker.CallTimes);
        }

        [Fact]
        public void TrimToLast1CallTimes_When2CallsExist_ReturnsMostRecentCallTime()
        {
            var now = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(now);

            var callTracker = new CallTracker();
            callTracker.CallWillHappenIn(200);
            callTracker.CallWillHappenIn(300);

            callTracker.TrimToLastXCallTimes(1);

            Assert.Single(callTracker.CallTimes);
            Assert.Equal(callTracker.CallTimes.Single(), now.AddMilliseconds(300));
        }
    }
}
