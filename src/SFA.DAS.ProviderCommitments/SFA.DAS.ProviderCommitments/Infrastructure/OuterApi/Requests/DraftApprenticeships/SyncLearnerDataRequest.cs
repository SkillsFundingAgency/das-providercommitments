namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;

public class SyncLearnerDataRequest(long providerId, long cohortId, long draftApprenticeshipId) : IPostApiRequest
{
    public string PostUrl => $"provider/{providerId}/unapproved/{cohortId}/apprentices/{draftApprenticeshipId}/sync-learner-data";
    public object Data { get; set; } = null;
}