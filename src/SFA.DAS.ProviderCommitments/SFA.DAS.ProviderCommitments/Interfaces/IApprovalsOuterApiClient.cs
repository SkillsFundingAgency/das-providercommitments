using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IApprovalsOuterApiClient
    {
        Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            long accountLegalEntityId,
            CancellationToken cancellationToken = default);

        Task<GetProviderAccountLegalEntitiesResponse> GetProviderAccountLegalEntities(
            int? ukprn,
            string operations,
            string accountHashedId);

        Task<GetHasPermissionResponse> GetHasPermission(
            long? ukPrn,
            long? accountLegalEntityId,
            Operation operation);
    }
}