namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetPriorLearningDataQueryRequest : IGetApiRequest
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public GetPriorLearningDataQueryRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
            ProviderId = providerId;
        }
        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/prior-learning-data";
    }
}