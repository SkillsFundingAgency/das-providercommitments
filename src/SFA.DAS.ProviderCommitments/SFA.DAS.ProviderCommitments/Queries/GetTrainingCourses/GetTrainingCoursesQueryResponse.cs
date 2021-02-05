using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryResponse
    {
        public TrainingProgramme[] TrainingCourses { get; set; }
    }
}