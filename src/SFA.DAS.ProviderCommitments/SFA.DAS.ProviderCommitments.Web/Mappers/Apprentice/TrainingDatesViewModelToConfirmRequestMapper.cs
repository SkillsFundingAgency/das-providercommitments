using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelToConfirmRequestMapper : IMapper<TrainingDatesViewModel, ConfirmRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public TrainingDatesViewModelToConfirmRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<ConfirmRequest> Map(TrainingDatesViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.EndDate = source.EndDate.MonthYear;
            cacheItem.StartDate = source.StartDate.Date.Value.ToString("MMyyyy");
            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);

            return new ConfirmRequest
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                CacheKey = source.CacheKey
            };
        }
    }
}
