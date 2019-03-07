using System;
using System.Net.Http;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.ProviderCommitments.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class CommitmentsApiRegistry : Registry
    {
        public CommitmentsApiRegistry()
        {
            For<ICommitmentsApiClient>().Use<CommitmentsApiClient>("", ctx =>
            {
                // TODO: there is no authentication in the client yet.
                var config = ctx.GetInstance<CommitmentsClientApiConfiguration>();
                var httpClient = new HttpClient {BaseAddress = new Uri(config.BaseUrl)};
                var restClient = new CommitmentsRestHttpClient(httpClient);
                return new CommitmentsApiClient(restClient);
            }).Singleton();
            
        }
    }
}
