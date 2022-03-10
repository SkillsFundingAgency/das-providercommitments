using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Net.Http;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CommitmentsOuterApiHttpClient : RestHttpClient, ICommitmentsOuterApiHttpClient
    {
        public CommitmentsOuterApiHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}