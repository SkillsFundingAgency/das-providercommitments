using System.Net.Http;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry
{
    public class LocalDevPasAccountApiClientFactory
    {
        private readonly PasAccountApiConfiguration _configuration;

        public LocalDevPasAccountApiClientFactory(PasAccountApiConfiguration configuration) => _configuration = configuration;

        public IPasAccountApiClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);
            var restHttpClient = new RestHttpClient(httpClient);
            return new PasAccountApiClient(restHttpClient);
        }
    }
}
