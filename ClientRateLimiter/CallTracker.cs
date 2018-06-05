using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientRateLimiter
{
    internal class CallTracker
    {
        private List<DateTime> _callTimes;

        public IEnumerable<DateTime> CallTimes
        {
            get
            {
                return _callTimes;
            }
        }

        public CallTracker()
        {
            _callTimes = new List<DateTime>();
        }

        /// <summary>
        /// Records when the next call is scheduled to occur
        /// </summary>
        /// <param name="milliseconds"></param>
        public void CallWillHappenIn(double milliseconds)
        {
            _callTimes.Add(ReferenceTime.UtcNow.AddMilliseconds(milliseconds));
        }

        public IEnumerable<DateTime> GetLastXCallTimes(int amount)
        {
            return _callTimes
                .OrderByDescending(x => x)
                .Take(amount);
        }

        /// <summary>
        /// Will remove all calls except the most recent, indicated by the amount of calls that should be kept
        /// </summary>
        /// <param name="amount"></param>
        public void TrimToLastXCallTimes(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount of calls must be zero or a postive value");
            }

            _callTimes = _callTimes
                .OrderByDescending(x => x)
                .Take(amount)
                .ToList();
        }
    }
}
