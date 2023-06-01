using System;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? GetFirstDayOfMonth(this DateTime? dateValue)
        {
            if (!dateValue.HasValue) return null;

            return new DateTime(dateValue.Value.Year, dateValue.Value.Month, 1).Date;
        }
    }
}
