﻿namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? GetFirstDayOfMonth(this DateTime? dateValue)
        {
            if (!dateValue.HasValue) return null;

            return new DateTime(dateValue.Value.Year, dateValue.Value.Month, 1).Date;
        }
        public static string ToGdsHumanisedDate(this DateTime date)
        {
            string ordinal;

            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    ordinal = "st";
                    break;
                case 2:
                case 22:
                    ordinal = "nd";
                    break;
                case 3:
                case 23:
                    ordinal = "rd";
                    break;
                default:
                    ordinal = "th";
                    break;
            }

            // Eg 12th January 2024
            return string.Format("{0}{1} {2:MMMM yyyy}", date.Day, ordinal, date);
        }
    }
}
