using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientRateLimiter
{
    /// <summary>
    /// Limits the number of calls that can be made within a certain time period
    /// </summary>
    public class StandardRateLimit : RateLimit
    {
        public StandardRateLimit(int amount, TimeSpan timeFrame) 
            : base(amount, timeFrame)
        {
        }

        internal override int GetNextAllowedCallTime(CallTracker callTracker)
        {
            var callTimes = callTracker.CallTimes
                .OrderByDescending(x => x)
                .Take(Amount)
                .ToList();

            if (!callTimes.Any())
            {
                return 0;
            }

            var earliestCallTime = callTimes.OrderBy(x => x).First();
            var untilNextCall = ReferenceTime.UtcNow.Subtract(earliestCallTime);

            var milliseconds = untilNextCall.TotalMilliseconds < 0 ? 0 : untilNextCall.TotalMilliseconds;

            return (int)Math.Ceiling(milliseconds);
        }
    }
}
