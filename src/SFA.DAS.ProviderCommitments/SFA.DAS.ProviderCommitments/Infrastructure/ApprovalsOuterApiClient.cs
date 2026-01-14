using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Account;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class ApprovalsOuterApiClient(IApprovalsOuterApiHttpClient client) : IApprovalsOuterApiClient
    {
        public Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            long accountLegalEntityId,
            CancellationToken cancellationToken = default)
        {
            return client.Get<ProviderCourseDeliveryModels>
                ($"providers/{providerId}/courses?trainingCode={courseCode}&accountLegalEntityId={accountLegalEntityId}",
                cancellationToken: cancellationToken);
        }

        public Task<GetHasPermissionResponse> GetHasPermission(long ukprn, long accountLegalEntityId)
        {
            return client.Get<GetHasPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}&accountLegalEntityId={accountLegalEntityId}");
        }

        public Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn)
        {
            return client.Get<GetHasRelationshipWithPermissionResponse>($"providerpermissions/has-relationship-with-permission?ukprn={ukprn}");
        }

        public Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int ukprn)
        {
            return client.Get<GetProviderAccountLegalEntitiesResponse>($"providerpermissions/account-provider-legal-entities?ukprn={ukprn}");
        }

        public Task<GetAccountResponse> GetAccount(string hashedAccountId)
        {
            return client.Get<GetAccountResponse>($"accounts/{hashedAccountId}");
        }

        public Task<GetSelectEmployerResponse> GetSelectEmployer(GetSelectEmployerRequest request)
        {
            return client.Get<GetSelectEmployerResponse>(request.GetUrl);
        }
    }
}
