using System;

namespace SFA.DAS.ProviderCommitments.Models
{
    public class MonthYearModel : DateModel
    {
        private readonly int _fixedDay;

        public MonthYearModel(string monthYear) : this(monthYear, DayAlignment.StartOfMonth)
        {
            // just call other constructor
        }

        public MonthYearModel(string monthYear, int fixedDay) : this(monthYear, fixedDay, DayAlignment.Fixed)
        {
        }

        public MonthYearModel(string monthYear, DayAlignment dayAlignment): this(monthYear, 1, dayAlignment)
        {
        }

        private MonthYearModel(string monthYear, int fixedDay, DayAlignment dayAlignment)
        {
            _fixedDay = fixedDay;
            DayAlignment = dayAlignment;
            SetFromMonthYear(monthYear);
        }

        public string MonthYear => $"{Month:D2}{Year:D4}";

        public DayAlignment DayAlignment { get; set; }

        public override int Day
        {
            get => DetermineDay();
            set => throw new InvalidOperationException($"Cannot set the day on a month-year structure. The day has been fixed at {base.Day}");
        }

        private int DetermineDay()
        {
            switch (DayAlignment)
            {
                case DayAlignment.Fixed: return _fixedDay;
                case DayAlignment.StartOfMonth: return 1;
                case DayAlignment.EndOfMonth: return DateTime.DaysInMonth(Year, Month);
            }

            throw new InvalidOperationException($"The value of for Day Alignment {DayAlignment} is not supported");
        }

        private void SetFromMonthYear(string monthYear)
        {
            int mmyyyyLength = "MMYYYY".Length;
            int myyyyLength = "MYYYY".Length;

            if (string.IsNullOrWhiteSpace(monthYear))
            {
                return;
            }

            if (monthYear.Length < myyyyLength ||
                monthYear.Length > mmyyyyLength ||
                !int.TryParse(monthYear, out _))
            {
                throw new ArgumentException("The month and year must be in the format mmyyyy or myyyy", nameof(monthYear));
            }

            var monthLength = monthYear.Length == myyyyLength ? 1 : 2;

            Year = int.Parse(monthYear.Substring(monthLength));
            Month = int.Parse(monthYear.Substring(0, monthLength));

            if (!IsValid)
            {
                throw new ArgumentException($"Either the month year {monthYear} is not valid or the day {Day} is not valid for this month.");
            }
        }
    }
}