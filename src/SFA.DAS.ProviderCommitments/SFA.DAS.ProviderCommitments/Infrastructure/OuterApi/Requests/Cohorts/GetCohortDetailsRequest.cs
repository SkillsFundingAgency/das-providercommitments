namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetCohortDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }

        public GetCohortDetailsRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}";
    }

    public class GetCohortDetailsResponse
    {
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public bool HasUnavailableFlexiJobAgencyDeliveryModel { get; set; }
    }
}
