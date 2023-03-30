using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Net.Http;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class OpenAiHttpClient : RestHttpClient, IOpenAiHttpClient
    {
        public OpenAiHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}