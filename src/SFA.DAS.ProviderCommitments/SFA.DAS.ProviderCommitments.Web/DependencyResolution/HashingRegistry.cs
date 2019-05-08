using SFA.DAS.ProviderCommitments.Services;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IPublicAccountIdHashingService>().Use<PublicAccountIdHashingService>();
            For<IPublicAccountLegalEntityIdHashingService>().Use<PublicAccountLegalEntityIdHashingService>();
        }
    }
}