using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.ProviderCommitments.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public interface IOpenAiHttpClientFactory
    {
        OpenAiHttpClient CreateClient();
    }

    public class OpenAiHttpClientFactory : IOpenAiHttpClientFactory
    {
        private readonly OpenAiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public OpenAiHttpClientFactory(OpenAiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public OpenAiHttpClient CreateClient()
        {
            var url =
                $"https://{_configuration.ResourceName}.openai.azure.com/openai/deployments/" +
                $"{_configuration.Model}/";

            var httpClient = new HttpClientBuilder()
                .WithHandler(new OpenAiHeadersHandler(_configuration))
                .WithDefaultHeaders()
                .WithLogging(_loggerFactory)
                .Build();
            httpClient.BaseAddress = new Uri(url);
            var restHttpClient = new OpenAiHttpClient(httpClient);
            return restHttpClient;
        }
    }

    public sealed class OpenAiHeadersHandler : DelegatingHandler
    {
        private readonly OpenAiConfiguration _config;

        public OpenAiHeadersHandler(OpenAiConfiguration config) => this._config = config;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add("api-key", this._config.ApiKey);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}