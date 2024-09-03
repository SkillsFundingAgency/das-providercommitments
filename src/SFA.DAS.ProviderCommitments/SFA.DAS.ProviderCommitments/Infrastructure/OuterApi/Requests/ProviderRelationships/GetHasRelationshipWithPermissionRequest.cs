namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships
{
    public record GetHasRelationshipWithPermissionRequest(long? UkPrn) : IGetApiRequest

    {
        public string GetUrl => $"providerpermissions/has-relationship-with-permission?ukprn={UkPrn}";
    }
}

