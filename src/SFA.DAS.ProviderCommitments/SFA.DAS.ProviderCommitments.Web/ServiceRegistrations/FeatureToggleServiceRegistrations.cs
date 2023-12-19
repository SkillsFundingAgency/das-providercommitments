using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.Handlers;
using SFA.DAS.Authorization.ProviderFeatures.Models;

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