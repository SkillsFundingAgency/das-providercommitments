
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships;

public record GetHasRelationshipWithPermissionRequest(long? UkPrn, Operation Operation) : IGetApiRequest
{
    public string GetUrl => $"providerpermissions/has-relationship-with-permission?ukprn={UkPrn}&operation={Operation}";
}