using System;
using System.Collections.Generic;
using System.Text;

namespace ClientRateLimiter
{
    public class RateLimiterConfiguration
    {
        public RateLimit BurstRate { get; set; }

        public RateLimit Rate { get; set; }

        public IRateLimiter Build()
        {
            return new RateLimiter(Rate, BurstRate);
        }
    }
}
