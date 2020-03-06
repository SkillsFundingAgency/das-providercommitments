using System;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class MonthYearStringExtensions
    {
        public static bool IsValidMonthYear(this string monthYear)
        {
            try
            {
                MonthYearModel dateModel = new MonthYearModel(monthYear);
                return dateModel.IsValid;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}