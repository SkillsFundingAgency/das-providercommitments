
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships;

public record GetHasPermissionRequest(long? UkPrn, long? AccountLegalEntityId, Operation Operation) : IGetApiRequest
{
    public string GetUrl => $"providerpermissions/has-permission?ukprn={UkPrn}&accountLegalEntityId={AccountLegalEntityId}&operation={Operation}";
}