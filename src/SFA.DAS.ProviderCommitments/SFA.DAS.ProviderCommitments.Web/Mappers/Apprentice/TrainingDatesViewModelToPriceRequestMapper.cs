using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelToPriceRequestMapper : IMapper<TrainingDatesViewModel, PriceRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public TrainingDatesViewModelToPriceRequestMapper(ICacheStorageService cacheStorage) =>
            _cacheStorage = cacheStorage;

        public async Task<PriceRequest> Map(TrainingDatesViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            cacheItem.StartDate = source.StartDate.MonthYear;
            cacheItem.EndDate = source.EndDate.MonthYear;
            cacheItem.EmploymentEndDate = source.EmploymentEndDate.MonthYear;

            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);

            return new PriceRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                CacheKey = source.CacheKey
            };
        }
    }
}