namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider
{
    public class GetProviderStatusDetails : IGetApiRequest
    {
        private readonly long _ukprn;

        public GetProviderStatusDetails(long ukprn)
        {
            _ukprn = ukprn;
        }

        public string GetUrl => $"provideraccounts/{_ukprn}";
    }
}
