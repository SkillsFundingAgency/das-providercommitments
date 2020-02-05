using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class MonthYearValidator : AbstractValidator<MonthYearModel>
    {
        public MonthYearValidator()
        {
            RuleFor(monthYear => monthYear).Must(monthYear => monthYear.IsValid).WithMessage(monthYear => $"The value {monthYear.SourceValue} is not valid for month-year");
        }
    }
}