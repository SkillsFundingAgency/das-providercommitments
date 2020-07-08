using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding
{
    public class ErrorSuppressModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            var converter = TypeDescriptor.GetConverter(bindingContext.ModelType);
            try
            {
                var result = converter.ConvertFrom(valueResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (ArgumentException)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                var errorSuppressBinderAttribute = (bindingContext.ModelMetadata as DefaultModelMetadata).Attributes.PropertyAttributes.FirstOrDefault(x => x.GetType() == typeof(ErrorSuppressBinderAttribute)) as ErrorSuppressBinderAttribute;
                bindingContext.ModelState.TryAddModelError(
                  errorSuppressBinderAttribute.PropertyName,
                  errorSuppressBinderAttribute.ErrorMessage);
               
            }
            return Task.CompletedTask;
        }
    }

    public class MonthYearModel1 : DateModel1
    {
        public MonthYearModel1(string monthYear)
        {
            SourceValue = monthYear;
            SetFromMonthYear(monthYear);
        }

        [JsonIgnore]
        public override int? Day
        {
            get => 1; // always use first day of month
            set => throw new InvalidOperationException("Cannot set the day on a month-year value");
        }

        public string MonthYear => $"{Month:D2}{Year:D4}";
        public string SourceValue { get; }

        public override bool HasValue => Month.HasValue || Year.HasValue;

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

    public class DateModel1
    {
        private DateTime? _currentValue;
        private int? _day;
        private int? _month;
        private int? _year;

        public DateModel1()
        {
            // provide a default constructor
        }

        public DateModel1(DateTime dateTime) : this()
        {
            _currentValue = dateTime;
            _day = dateTime.Day;
            _month = dateTime.Month;
            _year = dateTime.Year;
        }

        public virtual int? Day
        {
            get => _day;
            set
            {
                SetIfDifferent(_day, value, newValue => _day = newValue);
            }
        }

        public int? Month
        {
            get => _month;
            set
            {
                SetIfDifferent(_month, value, newValue => _month = newValue);
            }
        }

        public int? Year
        {
            get => _year;
            set
            {
                SetIfDifferent(_year, value, newValue => _year = newValue);
            }
        }

        public DateTime? Date => _currentValue ?? (_currentValue =
                                     IsValid ? new DateTime(Year.Value, Month.Value, Day.Value) : (DateTime?)null);

        public bool IsValid => HasValue && (IsValidDay(Day) && IsValidMonth(Month) && IsValidYear(Year) &&
                                            Day <= DateTime.DaysInMonth(Year.Value, Month.Value));

        public virtual bool HasValue => Day.HasValue || Month.HasValue || Year.HasValue;

        private bool IsValidDay(int? day)
        {
            return day.HasValue && day > 0 && day <= 31;
        }

        private bool IsValidMonth(int? month)
        {
            return month.HasValue && month > 0 && month <= 12;
        }

        private bool IsValidYear(int? year)
        {
            return year.HasValue && year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year;
        }

        private void SetIfDifferent(int? currentValue, int? newValue, Action<int?> change)
        {
            if (currentValue != newValue)
            {
                change(newValue);
                _currentValue = null;
            }
        }
    }
}
