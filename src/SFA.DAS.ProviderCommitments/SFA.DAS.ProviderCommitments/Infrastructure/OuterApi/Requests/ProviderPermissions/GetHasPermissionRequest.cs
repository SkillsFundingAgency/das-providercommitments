using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderPermissions;

public record GetHasPermissionRequest(long? UkPrn, long? AccountLegalEntityId, Operation Operation) : IGetApiRequest
{
    public string GetUrl => $"providerpermissions/has-permission?ukprn={UkPrn}&accountLegalEntityId={AccountLegalEntityId}&operation={Operation}";
}