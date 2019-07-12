using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.DependencyResolution;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class CommitmentsApiRegistry : Registry
    {
        public CommitmentsApiRegistry()
        {
            IncludeRegistry<CommitmentsApiClientRegistry>();

            For<ICommitmentsApiClientFactory>().Use("", x =>
            {
                var config = x.GetInstance<CommitmentsClientApiConfiguration>();
                var loggerFactory = x.GetInstance<ILoggerFactory>();
                return new CommitmentsApiClientFactory(config, loggerFactory);
            }).Singleton();
        }
    }
}
