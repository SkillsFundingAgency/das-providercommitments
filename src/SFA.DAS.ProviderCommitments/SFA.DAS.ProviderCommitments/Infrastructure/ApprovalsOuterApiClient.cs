using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;

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
            long accountLegalEntityId,
            CancellationToken cancellationToken = default)
        {
            return await _client.Get<ProviderCourseDeliveryModels>
                ($"providers/{providerId}/courses?trainingCode={courseCode}&accountLegalEntityId={accountLegalEntityId}",
                cancellationToken: cancellationToken);
        }

        public async Task<GetHasPermissionResponse> GetHasPermission(long ukprn, long accountLegalEntityId)
        {
            var result = await _client.Get<GetHasPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}&accountLegalEntityId={accountLegalEntityId}");
            return result;
        }

        public async Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn)
        {
            var result = await _client.Get<GetHasRelationshipWithPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}");
            return result;
        }

        public async Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int ukprn)
        {
            var result = await _client.Get<GetProviderAccountLegalEntitiesResponse>($"providerpermissions/account-provider-legal-entities?ukprn={ukprn}");
            return result;
        }
    }
}
