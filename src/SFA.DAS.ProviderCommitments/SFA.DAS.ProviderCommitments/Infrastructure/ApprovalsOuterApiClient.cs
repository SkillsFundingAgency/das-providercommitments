﻿using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class ApprovalsOuterApiClient : IApprovalsOuterApiClient
    {
        private readonly IApprovalsOuterApiHttpClient _client;

        public ApprovalsOuterApiClient(IApprovalsOuterApiHttpClient client)
        {
            _client = client;
        }

        public async Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            CancellationToken cancellationToken = default)
        {
            return await _client.Get<ProviderCourseDeliveryModels>
                ($"/approvals/providers/{providerId}/courses/{courseCode}",
                cancellationToken: cancellationToken);
        }
    }
}