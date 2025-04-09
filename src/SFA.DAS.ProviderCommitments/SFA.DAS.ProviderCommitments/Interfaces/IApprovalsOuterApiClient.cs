using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IApprovalsOuterApiClient
    {
        Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            long accountLegalEntityId,
            CancellationToken cancellationToken = default);

        Task<GetHasPermissionResponse> GetHasPermission(long ukprn, long accountLegalEntityId);

        Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int ukprn);

        Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn);
        Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn);
    }
}