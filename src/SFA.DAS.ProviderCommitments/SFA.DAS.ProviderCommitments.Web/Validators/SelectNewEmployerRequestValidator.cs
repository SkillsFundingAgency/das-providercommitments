using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Requests.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class SelectNewEmployerRequestValidator : AbstractValidator<SelectNewEmployerRequest>
        {
            public SelectNewEmployerRequestValidator()
            {
                RuleFor(x => x.ProviderId).GreaterThan(0);
                RuleFor(x => x.ApprenticeshipId).GreaterThan(0);
            }
        }
}
