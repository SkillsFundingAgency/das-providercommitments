using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using AddDraftApprenticeshipResponse = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.AddDraftApprenticeshipResponse;
using CreateCohortResponse = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.CreateCohortResponse;

namespace SFA.DAS.ProviderCommitments.Interfaces;

public interface IOuterApiService
{
    Task<GetAccountLegalEntityQueryResult> GetAccountLegalEntity(long publicAccountLegalEntityId);
    Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId);
    Task<GetCohortResult> GetCohort(long cohortId);
    Task<GetStandardResponse> GetStandardDetails(string courseCode);
    Task ValidateBulkUploadRequest(BulkUploadValidateApimRequest data);
    Task<BulkUploadAddAndApproveDraftApprenticeshipsResult> BulkUploadAddAndApproveDraftApprenticeships(BulkUploadAddAndApproveDraftApprenticeshipsRequest request);
    Task<GetBulkUploadAddDraftApprenticeshipsResult> BulkUploadDraftApprenticeships(BulkUploadAddDraftApprenticeshipsRequest request);
    Task CreateOverlappingTrainingDateRequest(CreateOverlappingTrainingDateApimRequest data);
    Task ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(ValidateDraftApprenticeshipApimRequest data);
    Task<ValidateUlnOverlapOnStartDateQueryResult> ValidateUlnOverlapOnStartDate(long providerId, string uln, string startDate, string endDate);
    Task ValidateChangeOfEmployerOverlap(ValidateChangeOfEmployerOverlapApimRequest data);
    Task<GetOverlapRequestQueryResult> GetOverlapRequest(long apprenticeshipId);
    Task UpdateDraftApprenticeship(long cohortId, long apprenticeshipId, UpdateDraftApprenticeshipApimRequest request);
    Task<AddDraftApprenticeshipResponse> AddDraftApprenticeship(long cohortId, AddDraftApprenticeshipApimRequest request);
    Task<CreateCohortResponse> CreateCohort(CreateCohortApimRequest request);
    Task<GetPriorLearningDataQueryResult> GetPriorLearningData(long providerId, long cohortId, long draftApprenticeshipId);
    Task<CreatePriorLearningDataResponse> UpdatePriorLearningData(long providerId, long cohortId, long draftApprenticeshipId, CreatePriorLearningDataRequest request);
    Task<GetPriorLearningSummaryQueryResult> GetPriorLearningSummary(long providerId, long cohortId, long apprenticeshipId);
    Task<GetCohortDetailsResponse> GetCohortDetails(long providerId, long cohortId);
    Task<PostApprenticeshipsCSVResponse> GetApprenticeshipsCSV(PostApprenticeshipsCSVRequest request);
    /// <summary>
    /// CONTRACT TO GET THE PROVIDER STATUS FROM THE OUTER API.
    /// </summary>
    /// <param name="ukprn">provider id or ukprn.</param>
    /// <returns>ProviderAccountResponse</returns>
    Task<ProviderAccountResponse> GetProviderStatus(long ukprn);
    Task<long> CreateFileUploadLog(long providerId, IFormFile attachment, List<CsvRecord> csvRecords);
    Task AddValidationMessagesToFileUploadLog(long providerId, long fileUploadLogId, List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError> errors);
    Task AddUnhandledExceptionToFileUploadLog(long providerId, long fileUploadLogId, string errorMessage);
    Task<bool> HasPermission(long ukprn, long? accountLegalEntityId);
    Task<bool> HasRelationshipWithPermission(long? ukprn);
    Task<bool> CanAccessCohort(long providerId, long cohortId);
    Task<bool> CanAccessApprenticeship(long providerId, long apprenticeshipId);
    Task<GetLearnerDetailsForProviderResponse> GetLearnerDetailsForProvider(long providerId, long? accountLegalEntityId, long? cohortId, string searchTerm, string sortColumn, bool sortDesc, int page, int? startMonth, int startYear);
    Task<GetLearnerSelectedResponse> GetLearnerSelected(long providerId, long learnerId);
    Task<GetRplRequirementsResponse> GetRplRequirements(long providerId, long cohortId, long draftApprenticeshipId, string courseCode);
    Task<ValidateEditApprenticeshipResponse> EditApprenticeship(long providerId, long apprenticeshipId, ValidateEditApprenticeshipRequest request);
    Task<ConfirmEditApprenticeshipResponse> ConfirmEditApprenticeship(long providerId, long apprenticeshipId, ConfirmEditApprenticeshipRequest request);
    Task<SyncLearnerDataResponse> SyncLearnerData(long providerId, long cohortId, long draftApprenticeshipId);   
    void DraftApprenticeshipSetReference(long providerId, long cohortId, long apprenticeshipId, DraftApprenticeshipSetReferenceApimRequest request);
    void DraftApprenticeshipAddEmail(long providerId, long cohortId, long apprenticeshipId, DraftApprenticeAddEmailApimRequest request);
}