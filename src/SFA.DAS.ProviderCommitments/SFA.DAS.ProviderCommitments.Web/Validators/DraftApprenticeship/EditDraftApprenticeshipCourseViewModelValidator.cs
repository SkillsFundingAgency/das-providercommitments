using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Validators.DraftApprenticeship
{
    public class EditDraftApprenticeshipCourseViewModelValidator : AbstractValidator<EditDraftApprenticeshipCourseViewModel>
    {
        public EditDraftApprenticeshipCourseViewModelValidator()
        {
            RuleFor(x => x.CourseCode).NotEmpty().WithMessage("You must select a training course");
        }
    }
}
