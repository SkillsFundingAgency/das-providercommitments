using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class CommitmentsApiClientServiceRegistrations
{
    public static IServiceCollection AddCommitmentsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddSingleton<ICommitmentsApiClientFactory>(x =>
        {
            var config = x.GetService<CommitmentsClientApiConfiguration>();
            var loggerFactory = x.GetService<ILoggerFactory>();
            
            if (configuration.UseLocalRegistry())
            {
                return new LocalDevApiClientFactory(config, loggerFactory);
            }
            
            return new CommitmentsApiClientFactory(config, loggerFactory);
        });
        
        services.AddTransient(provider => provider.GetRequiredService<ICommitmentsApiClientFactory>().CreateClient());

        return services;
    }
}