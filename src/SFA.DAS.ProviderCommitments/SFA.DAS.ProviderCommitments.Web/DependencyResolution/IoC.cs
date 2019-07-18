using SFA.DAS.Authorization.CommitmentPermissions.DependencyResolution;
using SFA.DAS.Authorization.DependencyResolution;
using SFA.DAS.Authorization.ProviderFeatures.DependencyResolution;
using SFA.DAS.Authorization.ProviderPermissions.DependencyResolution;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.Commitments.Shared.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Client.DependencyResolution;
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
            registry.IncludeRegistry<CommitmentsApiClientRegistry>();
            registry.IncludeRegistry<CommitmentPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<CommitmentsSharedRegistry>();
            registry.IncludeRegistry<MediatorRegistry>();
            registry.IncludeRegistry<ProviderFeaturesAuthorizationRegistry>();
            registry.IncludeRegistry<ProviderPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<DefaultRegistry>();
        }
    }
}