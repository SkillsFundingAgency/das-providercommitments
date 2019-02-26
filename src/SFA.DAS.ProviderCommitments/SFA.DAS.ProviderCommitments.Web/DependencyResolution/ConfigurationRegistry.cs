using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderCommitments.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderCommitments";

        public ConfigurationRegistry()
        {
            AddConfiguration<AuthenticationSettings>("AuthenticationSettings");
            AddConfiguration<ApprenticeshipInfoServiceConfiguration>("ApprenticeshipInfoServiceConfiguration");
        }

        private void AddConfiguration<T>(string name) where T : class
        {
            For<T>().Use(c => GetInstance<T>(c, name)).Singleton();
        }

        private static T GetInstance<T>(IContext context, string name)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var configSection = configuration.GetSection($"SFA.DAS.ProviderCommitments:{name}");
            var t = configSection.Get<T>();
            return t;
        }
    }
}
