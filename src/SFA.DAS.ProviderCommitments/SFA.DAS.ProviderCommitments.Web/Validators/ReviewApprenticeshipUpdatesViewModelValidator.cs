using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ReviewApprenticeshipUpdatesViewModelValidator : AbstractValidator<ReviewApprenticeshipUpdatesViewModel>
    {
        public ReviewApprenticeshipUpdatesViewModelValidator()
        {
            RuleFor(r => r.AcceptChanges).NotNull()
                .WithMessage("Confirm if you want to approve these changes");
        }
    }
}
