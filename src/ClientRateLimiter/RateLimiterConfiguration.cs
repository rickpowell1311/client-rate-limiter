﻿using System.Collections.Generic;

namespace ClientRateLimiter
{
    public class RateLimiterConfiguration
    {
        public static IRateLimiter Limitless
        {
            get
            {
                return new RateLimiter();
            }
        }

        private List<RateLimit> _rateLimits;

        public RateLimiterConfiguration()
        {
            _rateLimits = new List<RateLimit>();
        }

        public void AddRateLimit<T>(T rateLimit) where T : RateLimit
        {
            _rateLimits.Add(rateLimit);
        }

        public IRateLimiter BuildRateLimiter()
        {
            return new RateLimiter(_rateLimits.ToArray());
        }
    }
}
