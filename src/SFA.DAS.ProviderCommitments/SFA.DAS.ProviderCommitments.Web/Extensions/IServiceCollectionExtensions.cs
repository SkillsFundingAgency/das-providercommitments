using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authorisation;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class IWebHostBUilderExtensions
    {
        public static IWebHostBuilder ConfigureDasAppConfiguration(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration(c => c
                .AddAzureTableStorage("SFA.DAS.ProviderCommitments")
                .AddJsonFile("appsettings.json", true, true));
        }
    }

    public static class IServiceCollectionExtensions
    {
        private const string Key = "SFA.DAS.ProviderCommitments";

        public static IServiceCollection AddDasConfigurationSections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            
            services.Configure<ApprenticeshipInfoServiceConfiguration>(configuration.GetSection($"{Key}:ApprenticeshipInfoServiceConfiguration"));
            services.Configure<AuthenticationSettings>(configuration.GetSection($"{Key}:AuthenticationSettings"));
            services.Configure<PublicAccountIdHashingConfiguration>(configuration.GetSection($"{Key}:PublicAccountIdHashingConfiguration"));
            services.Configure<PublicAccountLegalEntityIdHashingConfiguration>(configuration.GetSection($"{Key}:PublicAccountLegalEntityIdHashingConfiguration"));
            services.Configure<AzureActiveDirectoryClientConfiguration>(configuration.GetSection($"{Key}:CommitmentsClientApiConfiguration"));
            return services;
        }

        public static IServiceCollection AddDasAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProviderMatch", policy => policy.Requirements.Add(new ProviderRequirement()));
            });

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IAuthorizationHandler, ProviderHandler>();

            return services;
        }
    }
}