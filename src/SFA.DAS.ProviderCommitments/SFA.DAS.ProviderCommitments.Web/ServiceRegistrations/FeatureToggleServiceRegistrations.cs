using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class FeatureToggleServiceRegistrations
{
    public static IServiceCollection AddProviderFeatures(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
        services.AddTransient<IFeatureTogglesService<ProviderFeatureToggle>, FeatureTogglesService<ProviderFeaturesConfiguration, ProviderFeatureToggle>>();
        
        return services;
    }
}