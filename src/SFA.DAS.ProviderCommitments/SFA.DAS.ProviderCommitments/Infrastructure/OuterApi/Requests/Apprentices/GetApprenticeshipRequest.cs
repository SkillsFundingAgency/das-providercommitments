namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetApprenticeshipRequest : IGetApiRequest
{
    private readonly long _apprenticeshipId;
    private readonly long _providerId;

    public GetApprenticeshipRequest(long apprenticeshipId, long providerId)
    {
        _apprenticeshipId = apprenticeshipId;
        _providerId = providerId;
    }

    public string GetUrl => $"/provider/{_providerId}/apprentices/{_apprenticeshipId}";
}