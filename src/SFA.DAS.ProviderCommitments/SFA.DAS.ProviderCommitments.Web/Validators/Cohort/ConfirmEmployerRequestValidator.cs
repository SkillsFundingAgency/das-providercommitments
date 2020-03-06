using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class ConfirmEmployerRequestValidator : AbstractValidator<ConfirmEmployerRequest>
    {
        public ConfirmEmployerRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(x => x.AccountLegalEntityId).GreaterThan(0);
        }
    }
}
