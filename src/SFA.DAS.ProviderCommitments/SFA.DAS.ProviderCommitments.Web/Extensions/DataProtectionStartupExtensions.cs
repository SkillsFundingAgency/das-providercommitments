﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IHostingEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var config = services.BuildServiceProvider().GetService<DataProtectionConnectionStrings>();

                if (config != null)
                {
                    var redisConnectionString = config.RedisConnectionString;
                    var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

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
