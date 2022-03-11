using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Configuration;
using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public interface IApprovalsOuterApiHttpClientFactory
    {
        ApprovalsOuterApiClient CreateClient();
    }

    public class ApprovalsOuterApiHttpClientFactory : IApprovalsOuterApiHttpClientFactory
    {
        private readonly ApprovalsOuterApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public ApprovalsOuterApiHttpClientFactory(ApprovalsOuterApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public ApprovalsOuterApiClient CreateClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithApimAuthorisationHeader(_configuration)
                .WithDefaultHeaders()
                .WithLogging(_loggerFactory)
                .Build();
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);
            var restHttpClient = new ApprovalsOuterApiHttpClient(httpClient);
            var apiClient = new ApprovalsOuterApiClient(restHttpClient);
            return apiClient;
        }
    }
}