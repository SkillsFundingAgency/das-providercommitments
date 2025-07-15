namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class PostConfirmEditApprenticeshipRequest(
    long providerId,
    long apprenticeshipId,
    ConfirmEditApprenticeshipRequest data)
    : IPostApiRequest
{
    public long ProviderId { get; } = providerId;
    public long ApprenticeshipId { get; } = apprenticeshipId;
    public string PostUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/edit/confirm";
    public object Data { get; set; } = data;
}