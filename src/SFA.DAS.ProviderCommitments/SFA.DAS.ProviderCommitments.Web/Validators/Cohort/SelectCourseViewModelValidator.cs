using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectCourseViewModelValidator : AbstractValidator<SelectCourseViewModel>
    {
        public SelectCourseViewModelValidator()
        {
            RuleFor(x => x.CourseCode).NotEmpty().WithMessage("You must select a training course");
        }
    }
}
