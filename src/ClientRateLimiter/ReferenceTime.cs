﻿using System;

namespace ClientRateLimiter
{
    public static class ReferenceTime
    {
        private static Func<DateTime> _utcDateTime;

        static ReferenceTime()
        {
            _utcDateTime = () => DateTime.UtcNow;
        }

        public static DateTime UtcNow => _utcDateTime();

        public static void FreezeAtUtc(DateTime utcDateTime)
        {
            _utcDateTime = () => utcDateTime;
        }

        public static void Unfreeze()
        {
            _utcDateTime = () => DateTime.UtcNow;
        }
    }
}
