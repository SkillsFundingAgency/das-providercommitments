using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectLearnerRecordViewModelMapper(IOuterApiService client)
    : IMapper<SelectLearnerRecordRequest, SelectLearnerRecordViewModel>
{
    public async Task<SelectLearnerRecordViewModel> Map(SelectLearnerRecordRequest source)
    {
        var response = await client.GetLearnerDetailsForProvider(source.ProviderId, source.AccountLegalEntityId,
            source.CohortId, source.SearchTerm, source.SortField, source.ReverseSort, source.Page, source.StartMonth, source.StartYear,source.CourseCode);

        var filterModel = new LearnerRecordsFilterModel()
        {
            ProviderId = source.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            CohortReference = source.CohortReference,
            CacheKey = source.CacheKey,
            ReservationId = source.ReservationId,
            TotalNumberOfLearnersFound = response.Total,
            PageNumber = response.Page,
            SortField = source.SortField,
            ReverseSort = source.ReverseSort,
            SearchTerm = source.SearchTerm,
            StartMonth = source.StartMonth.ToString(),
            StartYear = source.StartYear.ToString(),
            Courses = [new SelectListItem("All", ""),
            .. response.TrainingCourses.ToList()
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.CourseCode
                })],
            CourseCode = source.CourseCode,
        };

        var model = new SelectLearnerRecordViewModel
        {
            ProviderId = source.ProviderId,
            CohortReference = source.CohortReference,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            CacheKey = source.CacheKey,
            ReservationId = source.ReservationId,
            EmployerAccountName = response.EmployerName,
            Learners = response.Learners.ConvertAll(x => (LearnerSummary)x),
            LastIlrSubmittedOn = response.LastSubmissionDate,
            FilterModel = filterModel,
            FutureMonths = response.FutureMonths         
        };
        model.SortedByHeader();
        return model;
    }
}