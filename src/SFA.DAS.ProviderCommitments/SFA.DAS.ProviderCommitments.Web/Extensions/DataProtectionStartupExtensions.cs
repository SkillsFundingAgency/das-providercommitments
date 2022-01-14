using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ProviderCommitments.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var dataProtectionConfig = configuration.GetSection(ProviderCommitmentsConfigurationKeys.DataProtectionConnectionStrings).Get<DataProtectionConnectionStrings>();
                var redisConfig = configuration.GetSection(ProviderCommitmentsConfigurationKeys.RedisCache).Get<RedisConnectionSettings>();

                if (dataProtectionConfig != null && redisConfig != null)
                {
                    var redisConnectionString = redisConfig.RedisConnectionString;
                    var dataProtectionKeysDatabase = dataProtectionConfig.DataProtectionKeysDatabase;

                    var redis = ConnectionMultiplexer
                        .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                    services.AddDataProtection()
                        .SetApplicationName("das-providercommitments-web")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }

            return services;
        }
    }
}
