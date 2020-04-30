using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class EndDateViewModelValidator : AbstractValidator<EndDateViewModel>
    {
        public EndDateViewModelValidator()
        {
            RuleFor(x => x.ApprenticeshipHashedId)
                .NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId)
                .NotEmpty();
            RuleFor(x => x.ProviderId)
                .GreaterThan(0);
            RuleFor(x => x.AccountLegalEntityId)
                .GreaterThan(0);
            RuleFor(x => x.StartDate)
                .Must(field => field.IsValidMonthYear());
            RuleFor(x => x.EndDate)
                .Must(y => y.HasValue)
                .WithMessage("Enter the new training end date for this apprenticeship")
                .When(z => !z.EndDate.HasValue);
            RuleFor(x => x.EndDate)
                .Must(y => y.IsValid)
                .WithMessage("The end date is not valid")
                .When(z => z.EndDate.HasValue);
            RuleFor(x => x.EndDate)
                .Must((y, _) => y.EndDate.Date > (new MonthYearModel(y.StartDate).Date))
                .WithMessage("The new training end date cannot be before the stop date")
                .When(a => a.EndDate.HasValue && a.EndDate.IsValid);
        }
    }
}