
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;

public record GetCohortAccessRequest(Party Party, long PartyId, long CohortId): IGetApiRequest
{
    public string GetUrl => "authorization/CanAccessCohort";
}

public record GetCohortAccessResponse
{
    public bool HasCohortAccess { get; set; }
}
