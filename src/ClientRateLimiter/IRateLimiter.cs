using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientRateLimiter
{
    public interface IRateLimiter
    {
        IEnumerable<DateTime> CallHistory { get; }

        IEnumerable<RateLimit> RateLimits { get; }

        bool HasReachedLimit { get; }

        void Limit(Action limitedCall);

        T Limit<T>(Func<T> limitedCall);

        Task LimitAsync(Func<Task> limitedCall);

        Task<T> LimitAsync<T>(Func<Task<T>> limitedCall);
    }
}
