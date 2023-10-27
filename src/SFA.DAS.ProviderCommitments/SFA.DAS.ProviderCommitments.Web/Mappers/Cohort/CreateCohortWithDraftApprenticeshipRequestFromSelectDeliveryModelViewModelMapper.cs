using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<CreateCohortWithDraftApprenticeshipRequest> Map(SelectDeliveryModelViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
            cacheItem.DeliveryModel = source.DeliveryModel;
            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortWithDraftApprenticeshipRequest
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartMonthYear = source.StartMonthYear
            };
        }
    }
}