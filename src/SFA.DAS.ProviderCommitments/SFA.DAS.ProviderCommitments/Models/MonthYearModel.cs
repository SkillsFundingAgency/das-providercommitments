using System;
using System.Globalization;

namespace SFA.DAS.ProviderCommitments.Models
{
    public class MonthYearModel : DateModel
    {
        public MonthYearModel(string monthYear)
        {
            SourceValue = monthYear;
            SetFromMonthYear(monthYear);
        }

        public override int Day
        {
            get => 1; // always use first day of month
            set => throw new InvalidOperationException("Cannot set the day on a month-year value");
        }

        public string MonthYear => $"{Month:D2}{Year:D4}";

        public string SourceValue { get; }

        private void SetFromMonthYear(string monthYear)
        {
            if (string.IsNullOrWhiteSpace(monthYear))
            {
                return;
            }

            int mmyyyyLength = "MMYYYY".Length;
            int myyyyLength = "MYYYY".Length;

            if (monthYear.Length == myyyyLength || monthYear.Length == mmyyyyLength)
            {
                var monthLength = monthYear.Length - "YYYY".Length;

                SetValueIfValid(monthYear.Substring(monthLength), IsValidYear, year => Year = year);
                SetValueIfValid(monthYear.Substring(0, monthLength), IsValidMonth, month => Month = month);
            }
        }

        private void SetValueIfValid(string s, Func<int, bool> validator, Action<int> setter)
        {
            if (int.TryParse(s, out var intValue))
            {
                if (validator(intValue))
                {
                    setter(intValue);
                }
            }
        }
    }
}