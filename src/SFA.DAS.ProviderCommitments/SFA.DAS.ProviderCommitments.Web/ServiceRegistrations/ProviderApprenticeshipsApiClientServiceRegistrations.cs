using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Web.LocalDevRegistry;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class ProviderApprenticeshipsApiClientServiceRegistrations
{
    public static IServiceCollection AddProviderApprenticeshipsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPasAccountApiClient>(provider =>
        {
            var config = provider.GetService<PasAccountApiConfiguration>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            if (configuration.UseLocalRegistry())
            {
                var factory = new LocalDevPasAccountApiClientFactory(config);
                return factory.CreateClient();
            }
            else
            {
                var factory = new PasAccountApiClientFactory(config, loggerFactory);
                return factory.CreateClient();
            }
        });

        return services;
    }
}

internal class PasAccountApiClientFactory
{
    private readonly PasAccountApiConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public PasAccountApiClientFactory(PasAccountApiConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public IPasAccountApiClient CreateClient()
    {
        IHttpClientFactory httpClientFactory;

        if (IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret,
                _configuration.Tenant))
        {
            httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration, _loggerFactory);
        }
        else
        {
            httpClientFactory = new ManagedIdentityHttpClientFactory(_configuration, _loggerFactory);
        }

        var httpClient = httpClientFactory.CreateHttpClient();

        var restHttpClient = new RestHttpClient(httpClient);
        var apiClient = new PasAccountApiClient(restHttpClient);

        return apiClient;
    }

    private static bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        return !string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret) &&
               !string.IsNullOrWhiteSpace(tenant);
    }
}