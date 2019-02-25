using System;

namespace SFA.DAS.ProviderCommitments.Models
{
    /// <summary>
    ///     Encapsulates a date whilst exposing separate day, month and year properties that can be set independently.
    ///     The three elements are validated when set but may not be valid when used together, for example 31-Feb. In this
    ///     case accessing the <see cref="Date"/> will throw an exception. To avoid this check the <see cref="IsValid"/>
    ///     property before accessing <see cref="Date"/>.
    /// </summary>
    public class DateModel
    {
        private DateTime? _currentValue;
        private int _day;
        private int _month;
        private int _year;

        public DateModel()
        {
            // provide a default constructor
        }

        public DateModel(DateTime dateTime) : this()
        {
            _currentValue = dateTime;
            _day = dateTime.Day;
            _month = dateTime.Month;
            _year = dateTime.Year;
        }

        public virtual int Day
        {
            get => _day;
            set
            {
                AssertIsValid(nameof(Day), value, IsValidDay);
                SetIfDifferent(_day, value, newValue => _day = newValue);
            }
        }

        public int Month
        {
            get => _month;
            set
            {
                AssertIsValid(nameof(Month), value, IsValidMonth);
                SetIfDifferent(_month, value, newValue => _month = newValue);
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                AssertIsValid(nameof(Year), value, IsValidYear);
                SetIfDifferent(_year, value, newValue => _year = newValue);
            }
        }


        public DateTime? Date => _currentValue ?? (_currentValue = IsValid ? new DateTime(Year, Month, Day): (DateTime ?)null );


        public bool IsValid => IsValidDay(Day) && IsValidMonth(Month) && IsValidYear(Year) && Day <= DateTime.DaysInMonth(Year, Month);

        private void AssertIsValid(string property, int value, Func<int, bool> validator)
        {
            var isValid = validator(value);

            if (!isValid)
            {
                throw new ArgumentOutOfRangeException(property, $"The value {value} is out of range");
            }
        }

        private bool IsValidDay(int day)
        {
            return day > 0 && day <= 31;
        }


        private bool IsValidMonth(int month)
        {
            return month > 0 && month <= 12;
        }

        private bool IsValidYear(int year)
        {
            return year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year;
        }

        private void SetIfDifferent(int currentValue, int newValue, Action<int> change)
        {
            if (currentValue != newValue)
            {
                change(newValue);
                _currentValue = null;
            }
        }
    }
}