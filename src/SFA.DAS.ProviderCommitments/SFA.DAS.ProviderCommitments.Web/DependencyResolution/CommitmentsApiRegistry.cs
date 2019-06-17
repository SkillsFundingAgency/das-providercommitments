using SFA.DAS.Authorization.CommitmentPermissions;
using SFA.DAS.Authorization.CommitmentPermissions.Client;
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
                return new CommitmentsApiClientFactory(config);
            }).Singleton();
        }
    }
}
