namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostPriorLearningDataRequest(long providerId, long cohortId, long draftApprenticeshipId)
        : IPostApiRequest
    {
        public long CohortId { get; set; } = cohortId;
        public long DraftApprenticeshipId { get; set; } = draftApprenticeshipId;
        public long ProviderId { get; set; } = providerId;

        public object Data { get; set; }

        public string PostUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/prior-learning-data";
    }
}