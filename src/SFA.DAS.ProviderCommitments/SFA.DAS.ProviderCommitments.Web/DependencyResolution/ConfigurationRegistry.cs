using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.CommitmentPermissions.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using StructureMap;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            AddConfiguration<AuthenticationSettings>(ProviderCommitmentsConfigurationKeys.AuthenticationSettings);
            AddConfiguration<CommitmentsClientApiConfiguration>(ProviderCommitmentsConfigurationKeys.CommitmentsClientApiConfiguration);
            AddConfiguration<ApprovalsOuterApiConfiguration>(ProviderCommitmentsConfigurationKeys.ApprovalsOuterApiConfiguration);
            AddConfiguration<CommitmentPermissionsApiClientConfiguration>(ProviderCommitmentsConfigurationKeys.CommitmentsClientApiConfiguration);
            AddConfiguration<ProviderRelationshipsApiConfiguration>(ProviderCommitmentsConfigurationKeys.ProviderRelationshipsApiConfiguration);
            AddConfiguration<EncodingConfig>(ProviderCommitmentsConfigurationKeys.Encoding);
            AddConfiguration<ProviderFeaturesConfiguration>(ProviderCommitmentsConfigurationKeys.FeaturesConfiguration);
            AddConfiguration<ZenDeskConfiguration>(ProviderCommitmentsConfigurationKeys.ZenDeskConfiguration);
            AddConfiguration<DataProtectionConnectionStrings>(ProviderCommitmentsConfigurationKeys.DataProtectionConnectionStrings);
            AddConfiguration<BulkUploadFileValidationConfiguration>(ProviderCommitmentsConfigurationKeys.BulkUploadFileValidationConfiguration);
            AddConfiguration<ProviderSharedUIConfiguration>(ProviderCommitmentsConfigurationKeys.ProviderSharedUIConfiguration);
            AddConfiguration<BlobStorageSettings>(ProviderCommitmentsConfigurationKeys.BlobStorageSetttings);
            AddConfiguration<ApprovalsOuterApiConfiguration>(ProviderCommitmentsConfigurationKeys.ApprovalsOuterApiConfiguration);
            AddConfiguration<PasAccountApiConfiguration>(ProviderCommitmentsConfigurationKeys.ProviderAccountApiConfiguration);
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