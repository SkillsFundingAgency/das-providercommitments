using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class SelectLearnerRecordViewModelMapper(IOuterApiService client)
    : IMapper<SelectLearnerRecordRequest, SelectLearnerRecordViewModel>
{
    public async Task<SelectLearnerRecordViewModel> Map(SelectLearnerRecordRequest source)
    {
        var response = await client.GetLearnerDetailsForProvider(source.ProviderId, source.AccountLegalEntityId, source.CohortId,
            source.SearchTerm, source.SortField, source.ReverseSort, source.Page);

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
        };

        var model = new SelectLearnerRecordViewModel
        {
            ProviderId = source.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            CohortReference = source.CohortReference,
            CacheKey = source.CacheKey,
            ReservationId = source.ReservationId,
            EmployerAccountName = response.EmployerName,
            Learners = response.Learners.Select(x => (LearnerSummary) x).ToList(),
            LastIlrSubmittedOn = response.LastSubmissionDate,
            FilterModel = filterModel
        };
        model.SortedByHeader();
        return model;
    }
}