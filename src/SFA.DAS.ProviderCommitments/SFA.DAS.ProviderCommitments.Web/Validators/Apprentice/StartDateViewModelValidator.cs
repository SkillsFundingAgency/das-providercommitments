using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DatesViewModelValidator : AbstractValidator<StartDateViewModel>
    {
        public DatesViewModelValidator()
        {
            RuleFor(x => x.ApprenticeshipHashedId)
                .NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId)
                .NotEmpty();
            RuleFor(x => x.ProviderId)
                .GreaterThan(0);
            RuleFor(x => x.AccountLegalEntityId)
                .GreaterThan(0);
            RuleFor(x => x.StopDate)
                .NotEmpty();
            RuleFor(x => x.StartDate)
                .Must(y => y.HasValue)
                .WithMessage("Enter the new training start date for this apprenticeship")
                .When(z => !z.StartDate.HasValue);
            RuleFor(x => x.StartDate)
                .Must(y => y.IsValid)
                .WithMessage("The start date is not valid")
                .When(z => z.StartDate.HasValue);
            RuleFor(x => x.StartDate)
                .Must((y, z) => y.StartDate.Date >= y.StopDate)
                .WithMessage("The new training start date cannot be before the stop date")
                .When(a => a.StartDate.HasValue && a.StartDate.IsValid);
        }
    }
}