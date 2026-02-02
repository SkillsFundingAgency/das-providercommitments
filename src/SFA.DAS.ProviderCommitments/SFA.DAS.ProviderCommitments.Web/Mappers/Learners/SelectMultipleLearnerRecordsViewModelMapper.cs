using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsViewModelMapper(IOuterApiService client, ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsRequest, SelectMultipleLearnerRecordsViewModel>
{
    public async Task<SelectMultipleLearnerRecordsViewModel> Map(SelectMultipleLearnerRecordsRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        var response = await client.GetLearnerDetailsForProvider(cacheItem.ProviderId, cacheItem.AccountLegalEntityId,
            cacheItem.CohortId, cacheItem.SearchTerm, cacheItem.SortField, cacheItem.ReverseSort, source.Page,
            int.TryParse(cacheItem.StartMonth, out var m) ? m : null,
            int.Parse(cacheItem.StartYear));

        var filterModel = new MultipleLearnerRecordsFilterModel()
        {
            ProviderId = cacheItem.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = cacheItem.EmployerAccountLegalEntityPublicHashedId,
            CohortReference = cacheItem.CohortReference,
            CacheKey = cacheItem.CacheKey,
            ReservationId = cacheItem.ReservationId,
            TotalNumberOfLearnersFound = response.Total,
            PageNumber = response.Page,
            SortField = cacheItem.SortField,
            ReverseSort = cacheItem.ReverseSort,
            SearchTerm = cacheItem.SearchTerm,
            StartMonth = cacheItem.StartMonth,
            StartYear = cacheItem.StartYear
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