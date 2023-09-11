using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class GetEditApprenticeshipCourseRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetEditApprenticeshipCourseRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/edit/select-course";
    }

    public class GetEditApprenticeshipCourseResponse
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
