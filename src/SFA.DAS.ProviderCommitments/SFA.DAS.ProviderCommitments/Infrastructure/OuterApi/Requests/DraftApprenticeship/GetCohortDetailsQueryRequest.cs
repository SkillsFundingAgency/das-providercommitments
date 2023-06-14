
namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetCohortDetailsQueryRequest : IGetApiRequest
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }

        public GetCohortDetailsQueryRequest(long providerId, long cohortId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
        }
        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/";
    }
}
