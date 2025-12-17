using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Account;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class ApprovalsOuterApiClient(IApprovalsOuterApiHttpClient client) : IApprovalsOuterApiClient
    {
        public async Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            long accountLegalEntityId,
            CancellationToken cancellationToken = default)
        {
            return await client.Get<ProviderCourseDeliveryModels>
                ($"providers/{providerId}/courses?trainingCode={courseCode}&accountLegalEntityId={accountLegalEntityId}",
                cancellationToken: cancellationToken);
        }

        public async Task<GetHasPermissionResponse> GetHasPermission(long ukprn, long accountLegalEntityId)
        {
            var result = await client.Get<GetHasPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}&accountLegalEntityId={accountLegalEntityId}");
            return result;
        }

        public async Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn)
        {
            var result = await client.Get<GetHasRelationshipWithPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}");
            return result;
        }

        public async Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int ukprn)
        {
            var result = await client.Get<GetProviderAccountLegalEntitiesResponse>($"providerpermissions/account-provider-legal-entities?ukprn={ukprn}");
            return result;
        }

        public async Task<GetAccountResponse> GetAccount(string hashedAccountId)
        {
            var result = await client.Get<GetAccountResponse>($"accounts/{hashedAccountId}");
            return result;
        }

        public async Task<GetSelectEmployerResponse> GetSelectEmployer(GetSelectEmployerRequest request)
        {
            var result = await client.Get<GetSelectEmployerResponse>(request.GetUrl);
            return result;
        }
    }
}
