using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetAllCohortDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }

        public GetAllCohortDetailsRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}";
    }

    public class GetAllCohortDetailsResponse
    {
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public bool HasUnavailableFlexiJobAgencyDeliveryModel { get; set; }
        public IEnumerable<string> InvalidProviderCourseCodes { get; set; }
    }
}
