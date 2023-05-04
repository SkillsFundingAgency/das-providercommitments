using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipRedirectModelMapper : IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRedirectModel>
    {
        private readonly ICacheStorageService _cacheStorage;

        public AddDraftApprenticeshipRedirectModelMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<AddDraftApprenticeshipRedirectModel> Map(AddDraftApprenticeshipViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
            cacheItem.FirstName = source.FirstName;
            cacheItem.LastName = source.LastName;
            cacheItem.StartMonthYear = source.StartDate.MonthYear;
            cacheItem.Email = source.Email;
            cacheItem.Uln = source.Uln;
            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new AddDraftApprenticeshipRedirectModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                IsEdit = true
            };
        }
    }
}