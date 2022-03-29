using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Net.Http;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class ApprovalsOuterApiHttpClient : RestHttpClient, IApprovalsOuterApiHttpClient
    {
        public ApprovalsOuterApiHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}