using SFA.DAS.CommitmentsV2.Shared.Services;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using SFA.DAS.ProviderRelationships.Api.Client.Http;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ProviderRelationshipsApiClientServiceRegistrations
{
    public static IServiceCollection AddProviderRelationshipsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProviderRelationshipsApiClientFactory>(provider =>
        {
            var config = provider.GetService<ProviderRelationshipsApiConfiguration>();
           
            return new ProviderRelationshipsApiClientFactory(config);
        });

        if (configuration.GetValue<bool>("UseStubProviderRelationships"))
        {
            services.AddTransient<IProviderRelationshipsApiClient, StubProviderRelationshipsApiClient>();
        }
        else
        {
            services.AddTransient(provider => provider.GetRequiredService<IProviderRelationshipsApiClientFactory>().CreateApiClient());    
        }

        return services;
    }
}