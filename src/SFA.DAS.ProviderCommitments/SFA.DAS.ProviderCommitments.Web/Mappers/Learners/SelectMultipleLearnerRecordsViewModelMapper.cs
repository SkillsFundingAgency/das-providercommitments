using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
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

        var learnerRequest = new SelectLearnersRequest()
        {
            AccountLegalEntityId = cacheItem.AccountLegalEntityId,
            CohortId = cacheItem.CohortId,
            SearchTerm = cacheItem.SearchTerm,
            SortColumn = cacheItem.SortField,
            ReverseSort = cacheItem.ReverseSort,
            Page = source.Page,
            StartMonth = int.TryParse(cacheItem.StartMonth, out var m) ? m : null,
            StartYear = int.Parse(cacheItem.StartYear),
            CourseCode = cacheItem.CourseCode
        };

        var response = await client.GetLearnerDetailsForProvider(cacheItem.ProviderId, learnerRequest);

        var filterModel = new MultipleLearnerRecordsFilterModel()
        {
            ProviderId = cacheItem.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = cacheItem.EmployerAccountLegalEntityPublicHashedId,
            CohortReference = cacheItem.CohortReference,
            CacheKey = cacheItem.CacheKey,
            TotalNumberOfLearnersFound = response.Total,
            PageNumber = source.Page,
            SortField = cacheItem.SortField,
            ReverseSort = cacheItem.ReverseSort,
            SearchTerm = cacheItem.SearchTerm,
            StartMonth = cacheItem.StartMonth,
            StartYear = cacheItem.StartYear,
            CourseCode = cacheItem.CourseCode,
            Courses = [new SelectListItem("All", ""),
            .. response.TrainingCourses
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.CourseCode
                })]
        };

        var model = new SelectMultipleLearnerRecordsViewModel
        {
            ProviderId = cacheItem.ProviderId,
            CohortReference = cacheItem.CohortReference,
            EmployerAccountLegalEntityPublicHashedId = cacheItem.EmployerAccountLegalEntityPublicHashedId,
            CacheKey = source.CacheKey,
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