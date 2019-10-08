using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.ProviderRelationships.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.HealthChecks
{
    public class ProviderRelationshipsApiHealthCheck : IHealthCheck
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;

        public ProviderRelationshipsApiHealthCheck(IProviderRelationshipsApiClient providerRelationshipsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _providerRelationshipsApiClient.Ping(cancellationToken);
                
                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Degraded(exception.Message);
            }
        }
    }
}