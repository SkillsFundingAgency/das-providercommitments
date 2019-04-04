using SFA.DAS.HashingService;
using SFA.DAS.ProviderCommitments.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            Policies.Add<InjectHashingServiceByName>();

            RegisterNamedHashingService<PublicAccountIdHashingConfiguration>(HashingServiceNames.PublicAccountIdHashingService);
            RegisterNamedHashingService<PublicAccountLegalEntityIdHashingConfiguration>(HashingServiceNames.PublicAccountLegalEntityIdHashingService);
        }

        private void RegisterNamedHashingService<TConfigurationType>(string name) where TConfigurationType : HashingConfiguration
        {
            For<IHashingService>()
                .Add("", ctx =>
                {
                    var config = ctx.GetInstance<TConfigurationType>();
                    return new HashingService.HashingService(config.Alphabet, config.Salt);
                })
                .Named(name)
                .Singleton();
        }
    }
}
