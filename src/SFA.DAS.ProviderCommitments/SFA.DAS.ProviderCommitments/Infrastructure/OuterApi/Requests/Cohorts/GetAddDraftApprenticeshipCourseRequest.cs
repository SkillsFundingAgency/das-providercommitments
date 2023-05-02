using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetAddDraftApprenticeshipCourseRequest : IGetApiRequest
    {
        private readonly long _accountLegalEntityId;
        public long ProviderId { get; }
        
        public GetAddDraftApprenticeshipCourseRequest(long providerId, long accountLegalEntityId)
        {
            _accountLegalEntityId = accountLegalEntityId;
            ProviderId = providerId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/select-course?accountLegalEntityId={_accountLegalEntityId}";
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
