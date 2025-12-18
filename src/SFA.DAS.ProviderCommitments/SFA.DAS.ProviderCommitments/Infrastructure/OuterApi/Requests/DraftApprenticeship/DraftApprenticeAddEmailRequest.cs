namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

public class DraftApprenticeAddEmailRequest(long providerId, long cohortId, long draftApprenticeshipId) : IPostApiRequest
{
    public long CohortId { get; set; } = cohortId;
    public long DraftApprenticeshipId { get; set; } = draftApprenticeshipId;
    public long ProviderId { get; set; } = providerId;
    public string PostUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/email";
    public object Data { get; set; }
}
