using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetPriorLearningDataQueryRequest : IGetApiRequest
    {
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public long ProviderId { get; set; }

        public GetPriorLearningDataQueryRequest(long cohortId, long draftApprenticeshipId, long providerId)
        {
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
            ProviderId = providerId;
        }
        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/prior-learning-data";
    }
}
