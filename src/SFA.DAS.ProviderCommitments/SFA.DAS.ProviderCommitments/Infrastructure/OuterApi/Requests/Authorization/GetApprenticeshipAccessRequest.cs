
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;

public record GetApprenticeshipAccessRequest(Party Party, long PartyId, long ApprenticeshipId) : IGetApiRequest
{
    public string GetUrl => "api/authorization/access-apprenticeship";
}

public record GetApprenticeshipAccessResponse
{
    public bool HasApprenticeshipAccess { get; set; }
}