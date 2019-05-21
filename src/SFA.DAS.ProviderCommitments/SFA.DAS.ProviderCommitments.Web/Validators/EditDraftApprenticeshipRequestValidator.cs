using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class EditDraftApprenticeshipRequestValidator : AbstractValidator<EditDraftApprenticeshipRequest>
    {
        public EditDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.CohortId).NotEmpty();
            RuleFor(model => model.CohortReference).NotEmpty();
            RuleFor(model => model.DraftApprenticeshipId).NotEmpty();
            RuleFor(model => model.DraftApprenticeshipHashedId).NotEmpty();
        }
    }
}