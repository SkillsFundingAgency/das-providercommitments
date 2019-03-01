using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderCommitments.Configuration;

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
