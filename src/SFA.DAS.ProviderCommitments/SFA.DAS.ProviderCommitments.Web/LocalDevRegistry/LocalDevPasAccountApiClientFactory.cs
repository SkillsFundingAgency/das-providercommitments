using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry
{
    public class LocalDevPasAccountApiClientFactory
    {
        private readonly PasAccountApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public LocalDevPasAccountApiClientFactory(PasAccountApiConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IPasAccountApiClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);
            var restHttpClient = new RestHttpClient(httpClient);
            return new PasAccountApiClient(restHttpClient);
        }
    }
}
