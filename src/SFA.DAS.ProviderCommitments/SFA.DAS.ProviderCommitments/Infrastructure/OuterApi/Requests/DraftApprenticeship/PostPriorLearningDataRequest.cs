namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostPriorLearningDataRequest : IPostApiRequest
    {
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public long ProviderId { get; set; }

        public object Data { get; set; }

        public PostPriorLearningDataRequest(long providerId, long cohortId, long draftApprenticeshipId)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }
        public string PostUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/prior-learning-data";
    }
}