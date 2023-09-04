using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class GetAddDraftApprenticeshipCourseRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }

        public GetAddDraftApprenticeshipCourseRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/add/select-course";
    }

    public class GetAddDraftApprenticeshipCourseResponse
    {
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public bool IsMainProvider { get; set; }
        public IEnumerable<Standard> Standards { get; set; }

        public class Standard
        {
            public string CourseCode { get; set; }
            public string Name { get; set; }
        }
    }
}
