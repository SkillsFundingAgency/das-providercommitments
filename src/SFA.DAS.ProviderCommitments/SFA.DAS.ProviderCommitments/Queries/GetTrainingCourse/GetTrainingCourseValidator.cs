using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse
{
    public class GetTrainingCourseValidator : AbstractValidator<GetTrainingCourseRequest>
    {
        public GetTrainingCourseValidator()
        {
            RuleFor(request => request.CourseCode).NotEmpty().WithMessage("Course code must be supplied and not be empty");
        }
    }
}
