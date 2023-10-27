using System;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryRequest : IRequest<GetTrainingCoursesQueryResponse>
    {
        public GetTrainingCoursesQueryRequest()
        {
            IncludeFrameworks = true;
            EffectiveDate = null;
        }

        public bool IncludeFrameworks { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}