
namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetAllCohortDetailsQueryRequest : IGetApiRequest
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }

        public GetAllCohortDetailsQueryRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }
        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/all-cohort-details";
    }
}
