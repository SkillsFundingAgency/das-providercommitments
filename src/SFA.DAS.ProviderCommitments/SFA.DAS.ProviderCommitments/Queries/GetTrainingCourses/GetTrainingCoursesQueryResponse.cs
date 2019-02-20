using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryResponse
    {
        public List<ITrainingCourse> TrainingCourses { get; set; }
    }
}