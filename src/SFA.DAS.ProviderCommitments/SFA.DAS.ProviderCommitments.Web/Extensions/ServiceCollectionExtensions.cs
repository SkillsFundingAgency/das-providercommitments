using Microsoft.AspNetCore.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureDasAppConfiguration(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration(c => c
                .AddAzureTableStorage(
                    ProviderCommitmentsConfigurationKeys.Encoding,
                    ProviderCommitmentsConfigurationKeys.ProviderCommitments,
                    ProviderCommitmentsConfigurationKeys.PasAccountApiClient));
        }
    }
}