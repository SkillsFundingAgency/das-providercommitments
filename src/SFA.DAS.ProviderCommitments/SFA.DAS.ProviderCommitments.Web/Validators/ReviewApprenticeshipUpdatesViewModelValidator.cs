using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ReviewApprenticeshipUpdatesViewModelValidator : AbstractValidator<ReviewApprenticeshipUpdatesViewModel>
    {
        public ReviewApprenticeshipUpdatesViewModelValidator()
        {
            RuleFor(r => r.ApproveChanges).NotNull()
                .WithMessage("Confirm if you want to approve these changes")
                .When(z => z.IsValidCourseCode);

            RuleFor(r => r.ApproveAddStandardToTraining).NotNull()
                .WithMessage("You need to tell us if you want to add or reject the standard")
                .When(z => !z.IsValidCourseCode);
        }
    }
}
