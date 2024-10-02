using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.ProviderCommitments.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class DataProtectionStartupExtensions
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return services;
        }
        
        var config = configuration.GetSection(ProviderCommitmentsConfigurationKeys.DataProtectionConnectionStrings).Get<DataProtectionConnectionStrings>();

        if (config == null)
        {
            return services;
        }
        
        var redisConnectionString = config.RedisConnectionString;
        var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

        var redis = ConnectionMultiplexer
            .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName("das-provider")
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        return services;
    }
}