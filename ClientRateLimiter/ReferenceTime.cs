using System;

namespace ClientRateLimiter
{
    public static class ReferenceTime
    {
        private static Func<DateTime> _time;

        public static DateTime UtcNow => _time();

        public static void FreezeAt(DateTime time)
        {
            _time = () => time;
        }

        public static void Unfreeze()
        {
            _time = () => DateTime.UtcNow;
        }
    }
}
