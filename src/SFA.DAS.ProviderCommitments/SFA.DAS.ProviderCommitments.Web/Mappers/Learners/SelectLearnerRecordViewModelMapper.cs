using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectLearnerRecordViewModelMapper(IOuterApiService client, ILogger<SelectLearnerRecordViewModelMapper> logger)
    : IMapper<SelectLearnerRecordRequest, SelectLearnerRecordViewModel>
{
    public async Task<SelectLearnerRecordViewModel> Map(SelectLearnerRecordRequest source)
    {
        logger.LogInformation("Mapping SelectLearnerRecordViewModel for provider {ProviderId} and cohort {CohortReference}", source.ProviderId, source.CohortReference);
        logger.LogInformation("Request details: AccountLegalEntityId: {AccountLegalEntityId}, CohortId: {CohortId}, SearchTerm: {SearchTerm}, SortField: {SortField}, ReverseSort: {ReverseSort}, Page: {Page}, StartMonth: {StartMonth}, StartYear: {StartYear}",
            source.AccountLegalEntityId, source.CohortId, source.SearchTerm, source.SortField, source.ReverseSort, source.Page, source.StartMonth, source.StartYear);
        var response = await client.GetLearnerDetailsForProvider(source.ProviderId, source.AccountLegalEntityId,
            source.CohortId, source.SearchTerm, source.SortField, source.ReverseSort, source.Page, source.StartMonth, source.StartYear);

        logger.LogInformation("Received response for provider {ProviderId} and cohort {CohortReference}: Total: {Total}, Page: {Page}, PageSize: {PageSize}, TotalPages: {TotalPages}, AccountLegalEntityId: {AccountLegalEntityId}, EmployerName: {EmployerName}, LearnersCount: {LearnersCount}, FutureMonths: {FutureMonths}",
            source.ProviderId, source.CohortReference, response.Total, response.Page, response.PageSize, response.TotalPages, response.AccountLegalEntityId, response.EmployerName, response.Learners.Count, response.FutureMonths);


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
            StartYear = source.StartYear.ToString()
        };

        logger.LogInformation("Response for 1st record", response.Learners.FirstOrDefault()?.Course);

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