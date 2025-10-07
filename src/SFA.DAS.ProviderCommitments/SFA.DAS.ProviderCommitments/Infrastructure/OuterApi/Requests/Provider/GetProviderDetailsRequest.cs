namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;

public class GetProviderDetailsRequest(long ukprn) : IGetApiRequest
{
    public long _ukprn { get; } = ukprn;

    public string GetUrl => $"provideraccounts/providerStatus/{_ukprn}";
}

public class GetProviderDetailsResponse(int providerStatusTypeId)
{
    public int ProviderStatusTypeId { get; } = providerStatusTypeId;
}