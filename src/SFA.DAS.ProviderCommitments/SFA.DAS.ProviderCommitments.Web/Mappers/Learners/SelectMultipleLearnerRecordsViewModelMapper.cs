using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsViewModelMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsRequest, SelectMultipleLearnerRecordsViewModel>
{
    public async Task<SelectMultipleLearnerRecordsViewModel> Map(SelectMultipleLearnerRecordsRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        var response = await client.GetLearnerDetailsForProvider(cacheItem.ProviderId, cacheItem.AccountLegalEntityId,
            cacheItem.CohortId, cacheItem.Filter.SearchTerm, cacheItem.Filter.SortField, cacheItem.Filter.ReverseSort, cacheItem.Filter.PageNumber,
            int.TryParse(cacheItem.Filter.StartMonth, out var m) ? m : null,
            int.Parse(cacheItem.Filter.StartYear));

        var filterModel = new LearnerRecordsFilterModel()
        {
            ProviderId = cacheItem.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = cacheItem.EmployerAccountLegalEntityPublicHashedId,
            CohortReference = cacheItem.CohortReference,
            CacheKey = cacheItem.CacheKey,
            ReservationId = cacheItem.ReservationId,
            TotalNumberOfLearnersFound = response.Total,
            PageNumber = response.Page,
            SortField = cacheItem.Filter.SortField,
            ReverseSort = cacheItem.Filter.ReverseSort,
            SearchTerm = cacheItem.Filter.SearchTerm,
            StartMonth = cacheItem.Filter.StartMonth,
            StartYear = cacheItem.Filter.StartYear
        };

        var model = new SelectMultipleLearnerRecordsViewModel
        {
            ProviderId = cacheItem.ProviderId,
            CohortReference = cacheItem.CohortReference,
            EmployerAccountLegalEntityPublicHashedId = cacheItem.EmployerAccountLegalEntityPublicHashedId,
            CacheKey = source.CacheKey,
            ReservationId = cacheItem.ReservationId,
            EmployerAccountName = cacheItem.EmployerAccountName,
            Learners = response.Learners.ConvertAll(x => (LearnerSummary)x),
            LastIlrSubmittedOn = response.LastSubmissionDate,
            FilterModel = filterModel,
            FutureMonths = response.FutureMonths
        };
        model.SortedByHeader();
        return model;
    }
}