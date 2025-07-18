using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
 public class AddAnotherApprenticeshipRedirectModelMapper(
     ICacheStorageService cacheStorageService)
     : IMapper<BaseReservationsAddDraftApprenticeshipRequest, AddAnotherApprenticeshipRedirectModel>
 {
     public async Task<AddAnotherApprenticeshipRedirectModel> Map(BaseReservationsAddDraftApprenticeshipRequest source)
        {

            var cacheKey = Guid.NewGuid();
            var cacheItem = new AddAnotherApprenticeshipCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartMonthYear,
                CohortReference = source.CohortReference,
            };
            await cacheStorageService.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new AddAnotherApprenticeshipRedirectModel
            {
                CacheKey = cacheKey
            };
        }
    }
}
