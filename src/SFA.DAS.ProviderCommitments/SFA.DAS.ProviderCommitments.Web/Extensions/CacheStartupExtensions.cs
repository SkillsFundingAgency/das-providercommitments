using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class CacheStartupExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            var config = configuration.GetSection(ProviderCommitmentsConfigurationKeys.DataProtectionConnectionStrings).Get<DataProtectionConnectionStrings>();

            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(
                    options => { options.Configuration = config.RedisConnectionString; });
            }

            return services;
        }
    }
}
