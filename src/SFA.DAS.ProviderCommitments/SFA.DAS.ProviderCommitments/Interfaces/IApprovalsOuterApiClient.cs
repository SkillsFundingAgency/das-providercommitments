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

        Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(int ukprn);

        Task<GetHasRelationshipWithPermissionResponse> GetHasRelationshipWithPermission(long ukprn);
    }
}