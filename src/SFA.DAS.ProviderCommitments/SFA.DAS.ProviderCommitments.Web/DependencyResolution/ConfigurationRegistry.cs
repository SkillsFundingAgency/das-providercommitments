using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.ProviderFeatures;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            AddConfiguration<ApprenticeshipInfoServiceConfiguration>(ProviderCommitmentsConfigurationKeys.ApprenticeshipInfoServiceConfiguration);
            AddConfiguration<AuthenticationSettings>(ProviderCommitmentsConfigurationKeys.AuthenticationSettings);
            AddConfiguration<CommitmentsClientApiConfiguration>(ProviderCommitmentsConfigurationKeys.CommitmentsClientApiConfiguration);
            AddConfiguration<EncodingConfig>(ProviderCommitmentsConfigurationKeys.Encoding);
            AddConfiguration<ProviderFeaturesConfiguration>(ProviderCommitmentsConfigurationKeys.FeaturesConfiguration);
        }

        private void AddConfiguration<T>(string key) where T : class
        {
            For<T>().Use(c => GetConfiguration<T>(c, key)).Singleton();
        }

        private T GetConfiguration<T>(IContext context, string name)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var section = configuration.GetSection(name);
            var value = section.Get<T>();

            return value;
        }
    }
}