namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class PutEditApprenticeshipRequest(long providerId, long apprenticeshipId) : IPutApiRequest
{
    public string PutUrl => $"provider/{providerId}/apprentices/{apprenticeshipId}";
    public object Data { get; set; }
}