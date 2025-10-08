namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;

public class GetProviderDetailsRequest(long ukprn) : IGetApiRequest
{
    public string GetUrl => $"providers/{ukprn}/status";
}

public class GetProviderDetailsResponse(int providerStatusTypeId)
{
    public int ProviderStatusTypeId { get; } = providerStatusTypeId;
}