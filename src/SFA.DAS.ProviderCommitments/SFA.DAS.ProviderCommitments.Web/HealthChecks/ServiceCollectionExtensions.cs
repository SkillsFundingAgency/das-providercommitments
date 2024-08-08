using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDasHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<CommitmentsApiHealthCheck>("Commitments API Health Check");

            return services;
        }

        public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
        {
            return app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = (c, r) => c.Response.WriteJsonAsync(new
                {
                    r.Status,
                    r.TotalDuration,
                    Results = r.Entries.ToDictionary(
                        e => e.Key,
                        e => new
                        {
                            e.Value.Status,
                            e.Value.Duration,
                            e.Value.Description,
                            e.Value.Data
                        })
                })
            });
        }
    }
}