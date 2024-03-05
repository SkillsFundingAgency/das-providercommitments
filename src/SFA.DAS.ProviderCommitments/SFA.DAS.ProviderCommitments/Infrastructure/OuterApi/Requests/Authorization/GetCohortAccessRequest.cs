
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;

public record GetCohortAccessRequest(Party Party, long PartyId, long CohortId): IGetApiRequest
{
    public string GetUrl => "approvals/authorization/access-cohort";
}

public record GetCohortAccessResponse
{
    public bool HasCohortAccess { get; set; }
}
