using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.Authorization.CommitmentPermissions.Client;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.Http;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry
{
    public class LocalDevApiClientFactory : ICommitmentsApiClientFactory, ICommitmentPermissionsApiClientFactory
    {
        private readonly CommitmentsClientApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public LocalDevApiClientFactory(CommitmentsClientApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public ICommitmentsApiClient CreateClient()
        {
            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                var httpClientBuilder = new HttpClientBuilder();
                var httpClient = httpClientBuilder
               .WithDefaultHeaders()
               .Build();

                AddDevelopmentRole(httpClient, "Provider");

                httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);
                var byteArray = System.Text.Encoding.ASCII.GetBytes($"provider:password1234");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
                return new CommitmentsApiClient(restHttpClient);
            }
            else
            {
                throw new UnauthorizedAccessException("Not accessible");
            }
        }

        ICommitmentPermissionsApiClient ICommitmentPermissionsApiClientFactory.CreateClient()
        {
            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                var httpClientBuilder = new HttpClientBuilder();
                var httpClient = httpClientBuilder
               .WithDefaultHeaders()
               .Build();

                AddDevelopmentRole(httpClient, "Provider");
                
                httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

                var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
                return new CommitmentPermissionsApiClient(restHttpClient);
            }

            throw new UnauthorizedAccessException("Not accessible");
        }

        private static void AddDevelopmentRole(HttpClient httpClient, string role)
        {
            // some operations on the Commitments API require the role to be specificed
            // this would usually be done as part of the MI authorization, but when
            // MI is disabled this must be done using a custom header which will be read
            // by the Commitments API when running in Development mode and the role
            // will transfered into a claim
            httpClient.DefaultRequestHeaders.Add("Authorization",  role);
        }
    }
}