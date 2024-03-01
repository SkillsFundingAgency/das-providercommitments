using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class SelectPilotStatusViewModelMapper : IMapper<SelectPilotStatusRequest, SelectPilotStatusViewModel>
{
    private readonly ICacheStorageService _cacheStorageService;

    public SelectPilotStatusViewModelMapper(ICacheStorageService cacheStorageService) => _cacheStorageService = cacheStorageService;

    public async Task<SelectPilotStatusViewModel> Map(SelectPilotStatusRequest source)
    {
        var cacheItem = await _cacheStorageService.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);

        var result = new SelectPilotStatusViewModel
        {
            ProviderId = source.ProviderId,
            CacheKey = source.CacheKey,
            IsEdit = source.IsEdit
        };

        if (cacheItem.IsOnFlexiPaymentPilot.HasValue)
        {
            result.Selection = cacheItem.IsOnFlexiPaymentPilot.Value
                ? ChoosePilotStatusOptions.Pilot
                : ChoosePilotStatusOptions.NonPilot;
        }

        return result;
    }
}