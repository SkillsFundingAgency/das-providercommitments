using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse
{
    public class GetTrainingCourseRequest : IRequest<GetTrainingCourseResponse>
    {
        public string CourseCode { get; set; }
    }
}
