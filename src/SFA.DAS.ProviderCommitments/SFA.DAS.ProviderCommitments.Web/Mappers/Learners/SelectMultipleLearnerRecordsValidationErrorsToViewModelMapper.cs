using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsValidationErrorsToViewModelMapper(ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsValidationErrorsRequest, SelectMultipleLearnerRecordsValidationErrorsViewModel>
{
    public async Task<SelectMultipleLearnerRecordsValidationErrorsViewModel> Map(SelectMultipleLearnerRecordsValidationErrorsRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        return new SelectMultipleLearnerRecordsValidationErrorsViewModel
        {
            ProviderId = source.ProviderId,
            CacheKey = source.CacheKey,
            ValidationErrors = cacheItem.ValidationErrors
        };
    }
}