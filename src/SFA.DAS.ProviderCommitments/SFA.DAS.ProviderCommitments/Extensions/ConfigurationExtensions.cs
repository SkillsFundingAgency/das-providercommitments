using Microsoft.Extensions.Configuration;

namespace SFA.DAS.ProviderCommitments.Extensions;

public static class ConfigurationExtensions
{
    public static bool UseLocalRegistry(this IConfiguration configuration)
    {
        var isLocalRegistry = configuration.GetValue<bool?>("UseLocalRegistry");

        return isLocalRegistry != null && isLocalRegistry.Value;
    }
}