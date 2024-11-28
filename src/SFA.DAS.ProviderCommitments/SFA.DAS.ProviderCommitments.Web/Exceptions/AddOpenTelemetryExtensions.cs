using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class AddOpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

        if (!string.IsNullOrEmpty(connectionString))
        {
            services
                .AddOpenTelemetry()
                .UseAzureMonitor(options => options.ConnectionString = connectionString);
        }

        return services;
    }
}