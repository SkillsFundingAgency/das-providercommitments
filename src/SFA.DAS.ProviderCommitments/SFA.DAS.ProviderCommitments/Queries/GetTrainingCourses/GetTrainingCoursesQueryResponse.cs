using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryResponse
    {
        public ITrainingProgramme[] TrainingCourses { get; set; }
    }
}