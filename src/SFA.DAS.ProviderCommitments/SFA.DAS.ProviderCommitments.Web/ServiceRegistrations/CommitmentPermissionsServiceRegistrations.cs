using SFA.DAS.ProviderCommitments.Client;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Caching;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class CommitmentPermissionsServiceRegistrations
{
    public static IServiceCollection AddCommitmentPermissionsAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationHandler<ProviderAuthorizationHandler>(ResultsCacheType.EnableCaching);
        
        services.AddTransient<ICommitmentPermissionsApiClientFactory, CommitmentPermissionsApiClientFactory>();
        
        services.AddSingleton<IAuthorizationResultCacheConfigurationProvider, AuthorizationResultCacheConfigurationProvider>();
        services.AddSingleton(p => p.GetService<ICommitmentPermissionsApiClientFactory>().CreateClient());
        services.AddTransient<ICommitmentPermissionsApiClientFactory, CommitmentPermissionsApiClientFactory>();

        return services;
    }
}