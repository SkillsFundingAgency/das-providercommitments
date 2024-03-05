namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderPermissions;

public record GetHasPermissionRequest(long? UkPrn, long? AccountLegalEntityId, string Operation) : IGetApiRequest
{
    public string GetUrl => $"approvals/providerpermissions/has-permission?ukprn={UkPrn}&accountLegalEntityId={AccountLegalEntityId}&operation={Operation}";
}