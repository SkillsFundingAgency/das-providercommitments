using SFA.DAS.Authorization;
using SFA.DAS.Authorization.CommitmentPermissions;
using SFA.DAS.Authorization.CommitmentPermissions.Client;
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
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<CommitmentsApiRegistry>();
            registry.IncludeRegistry<CommitmentsPermissionsApiRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<EncodingRegistry>();
            registry.IncludeRegistry<MediatorRegistry>();
            registry.IncludeRegistry<AutoConfigurationRegistry>();
            registry.IncludeRegistry<ProviderPermissionsAuthorizationRegistry>();

            // This needs to go last as it replaces some of the default registrations in the package registartion above.
            registry.IncludeRegistry<DefaultRegistry>();
        }
    }
}
