﻿using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.CommitmentPermissions.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.ProviderFeatures.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.ProviderPermissions.DependencyResolution.StructureMap;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.CommitmentsV2.Api.Client.DependencyResolution;
using SFA.DAS.CommitmentsV2.Shared.DependencyInjection;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.DependencyResolution;
using SFA.DAS.ProviderRelationships.Api.Client.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public static class IoC
    {
        public static void Initialize(Registry registry, IConfiguration config)
        {
            registry.IncludeRegistry<AuthorizationRegistry>();
            registry.IncludeRegistry<AutoConfigurationRegistry>();
            registry.IncludeRegistry<CommitmentsApiClientRegistry>();
            registry.IncludeRegistry<ProviderRelationshipsApiClientRegistry>();
            registry.IncludeRegistry<CommitmentPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<CommitmentsSharedRegistry>();
            registry.IncludeRegistry<MediatorRegistry>();
            registry.IncludeRegistry<ProviderFeaturesAuthorizationRegistry>();
            registry.IncludeRegistry<ProviderPermissionsAuthorizationRegistry>();
            registry.IncludeRegistry<DefaultRegistry>();

            registry.IncludeRegistry(new PasAccountApiClientRegistry(c => config.GetSection(ProviderCommitmentsConfigurationKeys.ProviderAccountApiConfiguration).Get<PasAccountApiConfiguration>()));

            // Enable if you want to bypass MI locally - the 'Provider' role will
            // be added into a custom Authorization header which will be intercepted
            // by the Commitments API when running in Development and used to create a claim
            if (config["UseLocalRegistry"] != null && bool.Parse(config["UseLocalRegistry"]))
            {
                registry.IncludeRegistry<LocalRegistry>();
            }
        }
    }
}