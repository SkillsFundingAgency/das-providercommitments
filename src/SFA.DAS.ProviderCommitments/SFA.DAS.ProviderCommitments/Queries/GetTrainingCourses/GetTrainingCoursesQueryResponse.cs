using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryResponse
    {
        public ICourse[] TrainingCourses { get; set; }
    }
}