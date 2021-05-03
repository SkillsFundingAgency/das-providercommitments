using SFA.DAS.Authorization.CommitmentPermissions.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.ProviderFeatures.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.ProviderPermissions.DependencyResolution.StructureMap;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.CommitmentsV2.Shared.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Client.DependencyResolution;
using SFA.DAS.ProviderCommitments.DependencyResolution;
using StructureMap;
using SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution;

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
            registry.IncludeRegistry<PasAccountApiClientRegistry>();
            registry.IncludeRegistry<LocalDevRegistry.LocalDevRegistry>();
        }
    }
}