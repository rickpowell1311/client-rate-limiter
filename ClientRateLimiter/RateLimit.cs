using System;
using System.Collections.Generic;
using System.Text;

namespace ClientRateLimiter
{
    public abstract class RateLimit
    {
        public int Amount { get; }

        public TimeSpan TimeFrame { get; }

        public RateLimit(int amount, TimeSpan timeFrame)
        {
            Amount = amount;
            TimeFrame = timeFrame;
        }

        /// <summary>
        /// Gets the next allowed call time in milliseconds
        /// </summary>
        /// <returns></returns>
        internal abstract int GetNextAllowedCallTime(CallTracker callTracker);
    }
}
