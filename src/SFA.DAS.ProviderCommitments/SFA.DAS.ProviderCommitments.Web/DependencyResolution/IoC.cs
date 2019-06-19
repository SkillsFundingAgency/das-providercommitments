using SFA.DAS.Authorization;
using SFA.DAS.Authorization.ProviderFeatures;
using SFA.DAS.Authorization.CommitmentPermissions;
using SFA.DAS.Authorization.ProviderPermissions;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.ProviderCommitments.DependencyResolution;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public static class IoC
    {
        public static void Initialize(Registry registry)
        {
            registry.IncludeRegistry<AuthorizationRegistry>();
            registry.IncludeRegistry<AutoConfigurationRegistry>();
            registry.IncludeRegistry<CommitmentsApiRegistry>();
            registry.IncludeRegistry<CommitmentPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<EncodingRegistry>();
            registry.IncludeRegistry<MediatorRegistry>();
            registry.IncludeRegistry<ProviderFeaturesAuthorizationRegistry>();
            registry.IncludeRegistry<ProviderPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<DefaultRegistry>();
        }
    }
}