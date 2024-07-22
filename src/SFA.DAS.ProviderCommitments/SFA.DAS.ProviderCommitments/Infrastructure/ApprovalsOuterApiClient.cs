using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
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
        public Task<GetHasPermissionResponse> GetHasPermission(long? ukPrn, long? accountLegalEntityId, Operation operation)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int? ukprn, string operations, string accountHashedId)
        {
            throw new System.NotImplementedException();
        }
    }
}
