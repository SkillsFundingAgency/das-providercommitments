using SFA.DAS.ProviderCommitments.Client;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Caching;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class CommitmentPermissionsServiceRegistrations
{
    public static IServiceCollection AddCommitmentPermissionsAuthorization(this IServiceCollection services, bool useLocalRegistry)
    {
        if (useLocalRegistry)
        {
            services.AddTransient<ICommitmentPermissionsApiClientFactory, LocalDevApiClientFactory>();
        }
        else
        {
            services.AddTransient<ICommitmentPermissionsApiClientFactory, CommitmentPermissionsApiClientFactory>();
        }

        //services.AddAuthorizationHandler<ProviderAuthorizationHandler>(ResultsCacheType.EnableCaching);
        services.AddScoped<ProviderAuthorizationHandler>();
        
        services.AddTransient<ICommitmentPermissionsApiClientFactory, CommitmentPermissionsApiClientFactory>();
        
        services.AddSingleton<IAuthorizationResultCacheConfigurationProvider, AuthorizationResultCacheConfigurationProvider>();
        services.AddSingleton(p => p.GetService<ICommitmentPermissionsApiClientFactory>().CreateClient());
        
        return services;
    }
}