namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships
{
    public record GetHasPermissionRequest(long UkPrn, long accountLegalEntityId) : IGetApiRequest
    {
        public string GetUrl => $"providerpermissions/has-relationship-with-permission?ukprn={UkPrn}&accountLegalEntityId={accountLegalEntityId}";
    }
}

