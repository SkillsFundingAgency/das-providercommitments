using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class AddDraftApprenticeshipViewModelViewModelMapper : IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipViewModel>
{
    private readonly ICacheStorageService _cacheStorage;
    
    public AddDraftApprenticeshipViewModelViewModelMapper(ICacheStorageService cacheStorage)
    {
        _cacheStorage = cacheStorage;
    }

    public async Task<AddDraftApprenticeshipViewModel> Map(AddDraftApprenticeshipViewModel source)
    {
        var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
        
        var createCohortCacheItem = new CreateCohortCacheItem(source.CacheKey)
        {
            // TODO need to populate the ReservationId from somewhere
            ReservationId = source.ReservationId.Value,
            StartDate = source.StartDate.Date,
            EndDate = source.EndDate.Date,
            AccountLegalEntityId = source.AccountLegalEntityId,
        };
        
        await _cacheStorage.SaveToCache(cacheItem.CacheKey, createCohortCacheItem, 1);

        return source;
    }
}