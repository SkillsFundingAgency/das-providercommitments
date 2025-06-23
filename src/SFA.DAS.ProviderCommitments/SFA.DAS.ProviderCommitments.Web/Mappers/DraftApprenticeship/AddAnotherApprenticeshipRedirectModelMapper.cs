using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
 public class AddAnotherApprenticeshipRedirectModelMapper(
     ICacheStorageService cacheStorageService,
     ILogger<AddAnotherApprenticeshipRedirectModelMapper> logger)
     : IMapper<BaseReservationsAddDraftApprenticeshipRequest, AddAnotherApprenticeshipRedirectModel>
 {
     public async Task<AddAnotherApprenticeshipRedirectModel> Map(BaseReservationsAddDraftApprenticeshipRequest source)
        {
            logger.LogInformation("Returning AddAnotherApprenticeshipRedirectModel UseLearnerData {1}", source.UseLearnerData);

            var cacheKey = Guid.NewGuid();
            var cacheItem = new AddAnotherApprenticeshipCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartMonthYear,
                CohortReference = source.CohortReference,
                UseLearnerData = source.UseLearnerData
            };
            await cacheStorageService.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new AddAnotherApprenticeshipRedirectModel
            {
                CacheKey = cacheKey,
                UseLearnerData = source.UseLearnerData
            };
        }
    }
}
