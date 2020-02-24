using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Requests.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectEmployerRequestValidator : AbstractValidator<SelectEmployerRequest>
    {
        public SelectEmployerRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
        }
    }
}
