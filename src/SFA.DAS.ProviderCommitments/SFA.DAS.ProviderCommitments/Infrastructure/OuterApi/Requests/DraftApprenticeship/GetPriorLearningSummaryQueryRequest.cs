
namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetPriorLearningSummaryQueryRequest : IGetApiRequest
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public GetPriorLearningSummaryQueryRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }
        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/prior-learning-summary";
    }
}
