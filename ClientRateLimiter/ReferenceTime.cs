using System;
using System.Collections.Generic;
using System.Text;

namespace ClientRateLimiter
{
    public static class ReferenceTime
    {
        private static Func<DateTime> _time;

        public static DateTime UtcNow => _time();

        public static void Override(DateTime time)
        {
            _time = () => time;
        }

        public static void Reset()
        {
            _time = () => DateTime.UtcNow;
        }
    }
}
