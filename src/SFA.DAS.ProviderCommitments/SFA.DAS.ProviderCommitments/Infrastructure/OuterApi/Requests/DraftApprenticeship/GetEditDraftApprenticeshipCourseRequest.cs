using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class GetEditDraftApprenticeshipCourseRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public GetEditDraftApprenticeshipCourseRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/select-course";
    }

    public class GetEditDraftApprenticeshipCourseResponse
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
