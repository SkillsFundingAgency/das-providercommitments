using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public interface ICommitmentsOuterApiHttpClientFactory
    {
        CommitmentsOuterApiClient CreateClient();
    }

    public class CommitmentsOuterApiHttpClientFactory : ICommitmentsOuterApiHttpClientFactory
    {
        private readonly CommitmentsOuterApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public CommitmentsOuterApiHttpClientFactory(CommitmentsOuterApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public CommitmentsOuterApiClient CreateClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithApimAuthorisationHeader(_configuration)
                .WithDefaultHeaders()
                .WithLogging(_loggerFactory)
                .Build();
            var restHttpClient = new CommitmentsOuterApiHttpClient(httpClient, _loggerFactory);
            var apiClient = new CommitmentsOuterApiClient(restHttpClient);

            return apiClient;
        }
    }
}