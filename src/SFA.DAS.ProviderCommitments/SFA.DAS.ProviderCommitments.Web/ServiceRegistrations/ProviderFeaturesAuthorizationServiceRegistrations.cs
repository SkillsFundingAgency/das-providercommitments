using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.ProviderPermissions.Handlers;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ProviderFeaturesAuthorizationServiceRegistrations
{
    public static IServiceCollection AddProviderFeaturesAuthorization(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
        services.AddSingleton<IFeatureTogglesService<ProviderFeatureToggle>, FeatureTogglesService<ProviderFeaturesConfiguration, ProviderFeatureToggle>>();
        
        return services;
    }
}