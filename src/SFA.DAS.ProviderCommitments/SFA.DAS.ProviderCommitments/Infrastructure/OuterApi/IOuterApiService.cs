﻿using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using AddDraftApprenticeshipResponse = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.AddDraftApprenticeshipResponse;
using CreateCohortResponse = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.CreateCohortResponse;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
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
        Task<GetOverlapRequestQueryResult> GetOverlapRequest(long apprenticeshipId);
        Task UpdateDraftApprenticeship(long cohortId, long apprenticeshipId, UpdateDraftApprenticeshipApimRequest request);
        Task<AddDraftApprenticeshipResponse> AddDraftApprenticeship(long cohortId, AddDraftApprenticeshipApimRequest request);
        Task<CreateCohortResponse> CreateCohort(CreateCohortApimRequest request);
        Task<GetPriorLearningDataQueryResult> GetPriorLearningData(long providerId, long cohortId, long draftApprenticeshipId);
        Task<CreatePriorLearningDataResponse> UpdatePriorLearningData(long providerId, long cohortId, long draftApprenticeshipId, CreatePriorLearningDataRequest request);
        Task<GetPriorLearningSummaryQueryResult> GetPriorLearningSummary(long providerId, long cohortId, long apprenticeshipId);
        Task<GetCohortDetailsResponse> GetCohortDetails(long providerId, long cohortId);
        Task<long> CreateFileUploadLog(long providerId, IFormFile attachment, List<CsvRecord> csvRecords);
        Task AddValidationMessagesToFileUploadLog(long providerId, long fileUploadLogId, List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError> errors, ApimUserInfo UserInfo);
        Task AddUnhandledExceptionToFileUploadLog(long providerId, long fileUploadLogId, string errorMessage);
    }
}
 