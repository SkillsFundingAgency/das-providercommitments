using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class IWebHostBUilderExtensions
    {
        public static IWebHostBuilder ConfigureDasAppConfiguration(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration(c => c
                .AddAzureTableStorage("SFA.DAS.ProviderCommitments"));
        }
    }

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDasConfigurationSections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            return services;
        }
    }
}
