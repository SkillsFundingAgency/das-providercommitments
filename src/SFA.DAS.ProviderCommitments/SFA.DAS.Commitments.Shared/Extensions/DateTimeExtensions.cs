using System;

namespace SFA.DAS.Commitments.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToGdsFormat(this DateTime value)
        {
            return $"{value:d MMM yyyy}";
        }
        public static string ToGdsFormatWithoutDay(this DateTime value)
        {
            return $"{value:MMM yyyy}";
        }
    }
}
