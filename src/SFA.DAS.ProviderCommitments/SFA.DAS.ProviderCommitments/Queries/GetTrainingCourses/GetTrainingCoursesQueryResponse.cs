using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryResponse
    {
        public ITrainingProgramme[] TrainingProgrammes { get; set; }
    }
}