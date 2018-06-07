using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRateLimiter
{
    internal class RateLimiter : IRateLimiter
    {
        private static Object _lock;

        public CallTracker CallTracker { get; }

        public IEnumerable<DateTime> CallHistory
        {
            get
            {
                return CallTracker.CallHistory;
            }
        }

        public IEnumerable<RateLimit> RateLimits { get; }

        public bool HasReachedLimit
        {
            get
            {
                lock (_lock)
                {
                    var nextCallTime = RateLimits
                        .Max(l => l.GetNextAllowedCallTime(CallTracker));

                    return nextCallTime > 0;
                }
            }
        }

        public RateLimiter(params RateLimit[] rateLimits)
        {
            RateLimits = rateLimits.ToList();

            CallTracker = new CallTracker();
        }

        static RateLimiter()
        {
            _lock = new Object();
        }

        public void Limit(Action limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            Task.Delay(nextCallTime).Wait();
            limitedCall();
        }

        public T Limit<T>(Func<T> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            Task.Delay(nextCallTime).Wait();
            return limitedCall();
        }

        public async Task LimitAsync(Func<Task> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            await Task.Delay(nextCallTime);
            await limitedCall();
        }

        public async Task<T> LimitAsync<T>(Func<Task<T>> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            await Task.Delay(nextCallTime);
            return await limitedCall();
        }

        private int GetNextCallTime()
        {
            var nextCallTime = 0;

            if (RateLimits.Any())
            {
                lock (_lock)
                {
                    nextCallTime = RateLimits
                        .Max(l => l.GetNextAllowedCallTime(CallTracker));

                    CallTracker.CallWillHappenIn(nextCallTime);
                    CallTracker.TrimCallsForRateLimits(RateLimits);
                }
            }

            return nextCallTime;
        }
    }
}
