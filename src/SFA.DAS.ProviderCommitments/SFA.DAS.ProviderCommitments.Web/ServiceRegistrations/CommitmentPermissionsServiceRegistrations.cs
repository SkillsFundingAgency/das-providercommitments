using SFA.DAS.ProviderCommitments.Client;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Caching;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class CommitmentPermissionsServiceRegistrations
{
    public static IServiceCollection AddCommitmentPermissionsAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorizationHandler<ProviderAuthorizationHandler>(true)
            .AddSingleton<IAuthorizationResultCacheConfigurationProvider, AuthorizationResultCacheConfigurationProvider>()
            .AddSingleton(p => p.GetService<ICommitmentPermissionsApiClientFactory>().CreateClient())
            .AddTransient<ICommitmentPermissionsApiClientFactory, CommitmentPermissionsApiClientFactory>();
    }
}