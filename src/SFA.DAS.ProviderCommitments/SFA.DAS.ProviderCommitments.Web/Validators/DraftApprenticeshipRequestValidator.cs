using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class DraftApprenticeshipRequestValidator : AbstractValidator<DraftApprenticeshipRequest>
    {
        public DraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.CohortId).GreaterThan(0);
            RuleFor(model => model.CohortReference).NotEmpty();
            RuleFor(model => model.DraftApprenticeshipId).GreaterThan(0);
            RuleFor(model => model.DraftApprenticeshipHashedId).NotEmpty();
        }
    }
}