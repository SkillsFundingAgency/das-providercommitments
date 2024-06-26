﻿using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class FeatureToggleServiceRegistrations
{
    public static IServiceCollection AddProviderFeatures(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, ProviderFeaturesAuthorizationHandler>();
        services.AddTransient<IFeatureTogglesService<FeatureToggle>, FeatureTogglesService<ProviderFeaturesConfiguration, FeatureToggle>>();
        
        return services;
    }
}