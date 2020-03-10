using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DatesViewModelValidator : AbstractValidator<DatesViewModel>
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
            RuleFor(x => x.StartDate)
                .Must(y => y.HasValue)
                .WithMessage("Enter the new training start date for this apprenticeship");
            RuleFor(x => x.StartDate)
                .Must(y => y.IsValid)
                .WithMessage("The start date is not valid")
                .When(z => z.StartDate.HasValue);
        }
    }
}