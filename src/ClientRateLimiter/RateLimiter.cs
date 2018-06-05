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

        public IEnumerable<DateTime> CallTimes
        {
            get
            {
                return CallTracker.CallTimes;
            }
        }

        public IEnumerable<RateLimit> RateLimits { get; }

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
            Task.Delay(GetNextCallTime()).Wait();

            limitedCall();
        }

        public T Limit<T>(Func<T> limitedCall)
        {
            Task.Delay(GetNextCallTime()).Wait();

            return limitedCall();
        }

        public async Task LimitAsync(Func<Task> limitedCall)
        {
            await Task.Delay(GetNextCallTime());
            await limitedCall();
        }

        public async Task<T> LimitAsync<T>(Func<Task<T>> limitedCall)
        {
            await Task.Delay(GetNextCallTime());
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
                }
            }

            return nextCallTime;
        }
    }
}
