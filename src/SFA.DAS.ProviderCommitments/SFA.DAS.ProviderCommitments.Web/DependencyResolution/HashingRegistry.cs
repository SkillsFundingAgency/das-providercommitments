using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Services.Temp;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            RegisterNamedHashingService<PublicAccountIdHashingConfiguration>("publicAccountIdHashingService");
            RegisterNamedHashingService<PublicAccountLegalEntityIdHashingConfiguration>("publicAccountLegalEntityIdHashingService");
        }

        private void RegisterNamedHashingService<TConfigurationType>(string name) where TConfigurationType : HashingConfiguration
        {

            For<IHashingService>()
                .Use("", ctx =>
                {
                    var config = ctx.GetInstance<TConfigurationType>();
                    return new HashingService(config.Alphabet, config.Salt);
                })
                .Named(name)
                .Singleton();

            For<IHashingService>()
                .Use<IHashingService>()
                .Ctor<string>(name)
                .Named(name);
        }
    }
}
