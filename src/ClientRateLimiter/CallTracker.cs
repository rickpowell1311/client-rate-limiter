using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientRateLimiter
{
    internal class CallTracker
    {
        private List<DateTime> _callHistory;

        public IEnumerable<DateTime> CallHistory
        {
            get
            {
                return _callHistory;
            }
        }

        public CallTracker()
        {
            _callHistory = new List<DateTime>();
        }

        /// <summary>
        /// Records when the next call is scheduled to occur
        /// </summary>
        /// <param name="milliseconds"></param>
        public void CallWillHappenIn(double milliseconds)
        {
            _callHistory.Add(ReferenceTime.UtcNow.AddMilliseconds(milliseconds));
        }

        /// <summary>
        /// Will trim down the number of calls in the tracker to the amount required by these rate limits
        /// </summary>
        /// <param name="rateLimits"></param>
        public void TrimCallsForRateLimits(params RateLimit[] rateLimits)
        {
            TrimCallsForRateLimits(rateLimits.AsEnumerable());
        }

        /// <summary>
        /// Will trim down the number of calls in the tracker to the amount required by these rate limits
        /// </summary>
        /// <param name="rateLimits"></param>
        public void TrimCallsForRateLimits(IEnumerable<RateLimit> rateLimits)
        {
            if (!rateLimits.Any())
            {
                return;
            }

            var numberOfCallsToKeep = rateLimits.Max(rl => rl.Amount);
            TrimToMostRecentCalls(numberOfCallsToKeep);
        }

        private void TrimToMostRecentCalls(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount of calls must be zero or a postive value");
            }

            _callHistory = _callHistory
               .OrderByDescending(x => x)
               .Take(amount)
               .ToList();
        }
    }
}
