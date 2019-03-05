using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Services;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            RegisterPublicAccountIdHashingService();
            RegisterPublicAccountLegalEntityIdHashingService();
        }

        private void RegisterPublicAccountIdHashingService()
        {
            For<IPublicAccountIdHashingService>().Use("", ctx =>
            {
                var config = ctx.GetInstance<PublicAccountIdHashingConfiguration>();
                return new PublicAccountIdHashingService(config);
            }).Singleton();
        }

        private void RegisterPublicAccountLegalEntityIdHashingService()
        {
            For<IPublicAccountLegalEntityIdHashingService>().Use("", ctx =>
            {
                var config = ctx.GetInstance<PublicAccountLegalEntityIdHashingConfiguration>();
                return new PublicAccountLegalEntityIdHashingService(config);
            }).Singleton();
        }
    }
}
