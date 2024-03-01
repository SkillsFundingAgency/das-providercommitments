using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ApprovalsOuterApiClientServiceRegistrations
{
    public static IServiceCollection AddApprovalsOuterApiClient(this IServiceCollection services)
    {
       services.AddSingleton<IApprovalsOuterApiHttpClientFactory>(x =>
        {
            var config = x.GetService<ApprovalsOuterApiConfiguration>();
            var loggerFactory = x.GetService<ILoggerFactory>();
            
            return new ApprovalsOuterApiHttpClientFactory(config, loggerFactory);
        });
        
        services.AddSingleton<IApprovalsOuterApiClient>(provider => provider.GetRequiredService<IApprovalsOuterApiHttpClientFactory>().CreateClient());

        return services;
    }
}