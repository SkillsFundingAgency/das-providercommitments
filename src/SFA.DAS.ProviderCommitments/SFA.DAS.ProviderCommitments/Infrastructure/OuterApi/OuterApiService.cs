﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public class OuterApiService : IOuterApiService
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly IAuthenticationServiceForApim _authenticationService;

    public OuterApiService(IOuterApiClient outerApiClient, IAuthenticationServiceForApim authenticationService)
    {
        _outerApiClient = outerApiClient;
        _authenticationService = authenticationService;
    }

    public async Task<BulkUploadAddAndApproveDraftApprenticeshipsResult> BulkUploadAddAndApproveDraftApprenticeships(BulkUploadAddAndApproveDraftApprenticeshipsRequest data)
    {
        try
        {
            return await _outerApiClient.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(new PostBulkUploadAddAndApproveDraftApprenticeshipsRequest(data));
        }
        catch (CommitmentsApiBulkUploadModelException ex)
        {
            if (data.FileUploadLogId != null)
                await AddValidationMessagesToFileUploadLog(data.ProviderId, data.FileUploadLogId.Value, ex.Errors);
            throw;
        }
        catch (Exception ex)
        {
            if (data.FileUploadLogId != null)
                await AddUnhandledExceptionToFileUploadLog(data.ProviderId, data.FileUploadLogId.Value, ex.Message);
            throw;
        }
    }

    public async Task<GetBulkUploadAddDraftApprenticeshipsResult> BulkUploadDraftApprenticeships(BulkUploadAddDraftApprenticeshipsRequest data)
    {
        try
        {
            return await _outerApiClient.Post<GetBulkUploadAddDraftApprenticeshipsResult>(
                new PostBulkUploadAddDraftApprenticeshipsRequest(data));
        }
        catch (CommitmentsApiBulkUploadModelException ex)
        {
            if (data.FileUploadLogId != null)
                await AddValidationMessagesToFileUploadLog(data.ProviderId, data.FileUploadLogId.Value, ex.Errors);
            throw;
        }
        catch (Exception ex)
        {
            if (data.FileUploadLogId != null)
                await AddUnhandledExceptionToFileUploadLog(data.ProviderId, data.FileUploadLogId.Value, ex.Message);
            throw;
        }
    }

    public async Task<GetAccountLegalEntityQueryResult> GetAccountLegalEntity(long publicAccountLegalEntityId)
    {
        return await _outerApiClient.Get<GetAccountLegalEntityQueryResult>(new GetAccountLegalEntityRequest(publicAccountLegalEntityId));
    }

    public async Task<GetCohortResult> GetCohort(long cohortId)
    {
        return await _outerApiClient.Get<GetCohortResult>(new GetCohortRequest(cohortId));
    }

    public async Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId)
    {
        return await _outerApiClient.Get<GetDraftApprenticeshipsResult>(new GetDraftApprenticeshipsRequest(cohortId));
    }

    public async Task<GetStandardResponse> GetStandardDetails(string courseCode)
    {
        return await _outerApiClient.Get<GetStandardResponse>(new GetStandardDetailsRequest(courseCode));
    }

    public async Task ValidateBulkUploadRequest(BulkUploadValidateApimRequest data)
    {
        await _outerApiClient.Post<object>(new PostValidateBulkUploadDataRequest(data));
    }

    public async Task CreateOverlappingTrainingDateRequest(CreateOverlappingTrainingDateApimRequest data)
    {
        await _outerApiClient.Post<CreateOverlappingTrainingDateResponse>(new PostCreateOveralappingTrainingDateRequest(data));
    }

    public async Task ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(ValidateDraftApprenticeshipApimRequest data)
    {
        await _outerApiClient.Post<object>(new PostValidateDraftApprenticeshipforOverlappingTrainingDateRequest(data));
    }

    public async Task<ValidateUlnOverlapOnStartDateQueryResult> ValidateUlnOverlapOnStartDate(long providerId, string uln, string startDate, string endDate)
    {
        return await _outerApiClient.Get<ValidateUlnOverlapOnStartDateQueryResult>(new ValidateUlnOverlapOnStartDateQueryRequest(providerId, uln, startDate, endDate));
    }

    public async Task ValidateChangeOfEmployerOverlap(ValidateChangeOfEmployerOverlapApimRequest data)
    {
        await _outerApiClient.Post<object>(new PostValidateChangeOfEmployerOverlapRequest(data));
    }

    public async Task<GetOverlapRequestQueryResult> GetOverlapRequest(long apprenticeshipId)
    {
        return await _outerApiClient.Get<GetOverlapRequestQueryResult>(new GetOverlapRequestQueryRequest(apprenticeshipId));
    }

    public async Task UpdateDraftApprenticeship(long cohortId, long apprenticeshipId, UpdateDraftApprenticeshipApimRequest request)
    {
        await _outerApiClient.Put<object>(new PutUpdateDraftApprenticeshipRequest(cohortId, apprenticeshipId) { Data = request });
    }

    public async Task<AddDraftApprenticeshipResponse> AddDraftApprenticeship(long cohortId, AddDraftApprenticeshipApimRequest request)
    {
        return await _outerApiClient.Post<AddDraftApprenticeshipResponse>(new PostAddDraftApprenticeshipRequest(cohortId) { Data = request });
    }

    public async Task<CreateCohortResponse> CreateCohort(CreateCohortApimRequest request)
    {
        return await _outerApiClient.Post<CreateCohortResponse>(new PostCreateCohortRequest { Data = request });
    }

    public async Task<GetPriorLearningDataQueryResult> GetPriorLearningData(long providerId, long cohortId, long draftApprenticeshipId)
    {
        return await _outerApiClient.Get<GetPriorLearningDataQueryResult>(new GetPriorLearningDataQueryRequest(providerId, cohortId, draftApprenticeshipId));
    }

    public async Task<CreatePriorLearningDataResponse> UpdatePriorLearningData(long providerId, long cohortId, long draftApprenticeshipId, CreatePriorLearningDataRequest request)
    {
        return await _outerApiClient.Post<CreatePriorLearningDataResponse>(new PostPriorLearningDataRequest(providerId, cohortId, draftApprenticeshipId) { Data = request });
    }

    public async Task<GetPriorLearningSummaryQueryResult> GetPriorLearningSummary(long providerId, long cohortId, long draftApprenticeshipId)
    {
        return await _outerApiClient.Get<GetPriorLearningSummaryQueryResult>(new GetPriorLearningSummaryQueryRequest(providerId, cohortId, draftApprenticeshipId));
    }

    public async Task<GetCohortDetailsResponse> GetCohortDetails(long providerId, long cohortId)
    {
        return await _outerApiClient.Get<GetCohortDetailsResponse>(new GetCohortDetailsRequest(providerId, cohortId));
    }

    public async Task<PostApprenticeshipsCSVResponse> GetApprenticeshipsCSV(PostApprenticeshipsCSVRequest request)
    {
      return await _outerApiClient.Post<PostApprenticeshipsCSVResponse>(request);
    }

    // <inherit-doc />
    public async Task<ProviderAccountResponse> GetProviderStatus(long ukprn)
    {
        return await _outerApiClient.Get<ProviderAccountResponse>(new GetProviderStatusDetails(ukprn));
    }

    public async Task<long> CreateFileUploadLog(long providerId, IFormFile attachment, List<CsvRecord> csvRecords)
    {
        var rplCount = csvRecords.Count(x => x.RecognisePriorLearning != null && (x.RecognisePriorLearning == "1" ||
                                                                                  x.RecognisePriorLearning.Equals("True", StringComparison.InvariantCultureIgnoreCase) ||
                                                                                  x.RecognisePriorLearning.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)));

        var request = new FileUploadLogRequest
        {
            ProviderId = providerId,
            Filename = attachment.FileName,
            RplCount = rplCount,
            RowCount = csvRecords.Count,
            FileContent = await ReadFormFileAsync(attachment),
            UserInfo = GetUserInfo()
        };

        var response = await _outerApiClient.Post<FileUploadLogResponse>(new PostFileUploadLogRequest(request));
        return response.LogId;
    }

    public async Task AddValidationMessagesToFileUploadLog(long providerId, long fileUploadLogId, List<BulkUploadValidationError> errors)
    {
        var content = new FileUploadUpdateLogWithErrorContentRequest
        {
            ProviderId = providerId,
            ErrorContent = "Validation failure \r\n" + JsonConvert.SerializeObject(errors),
            UserInfo = GetUserInfo()
        };

        await _outerApiClient.Put<object>(new PutFileUploadUpdateLogRequest(fileUploadLogId, content));
    }

    public async Task AddUnhandledExceptionToFileUploadLog(long providerId, long fileUploadLogId, string errorMessage)
    {
        var content = new FileUploadUpdateLogWithErrorContentRequest
        {
            ProviderId = providerId,
            ErrorContent = "Unhandled exception \r\n" + errorMessage,
            UserInfo = GetUserInfo()
        };

        await _outerApiClient.Put<object>(new PutFileUploadUpdateLogRequest(fileUploadLogId, content));
    }

    public async Task<bool> HasPermission(long ukprn, long? accountLegalEntityId)
    {
        var content = new GetHasPermissionRequest(ukprn, accountLegalEntityId.GetValueOrDefault());
        var response = await _outerApiClient.Get<GetHasPermissionResponse>(content);
        return response.HasPermission;
    }

    public async Task<bool> HasRelationshipWithPermission(long? ukprn)
    {
        var content = new GetHasRelationshipWithPermissionRequest(ukprn);

        var response = await _outerApiClient.Get<GetHasPermissionResponse>(content);

        return response.HasPermission;
    }

    public async Task<bool> CanAccessApprenticeship(long providerId, long apprenticeshipId)
    {
        var content = new GetApprenticeshipAccessRequest(Party.Provider, providerId, apprenticeshipId);

        var response = await _outerApiClient.Get<GetApprenticeshipAccessResponse>(content);

        return response.HasApprenticeshipAccess;
    }

    public async Task<GetLearnerDetailsForProviderResponse> GetLearnerDetailsForProvider(long providerId, long? accountLegalEntityId, long? cohortId, string searchTerm, string sortColumn, bool sortDesc, int page)
    {
        var request = new GetLearnerDetailsForProviderRequest(providerId, accountLegalEntityId, cohortId, searchTerm, sortColumn, sortDesc, page);

        var response = await _outerApiClient.Get<GetLearnerDetailsForProviderResponse>(request);

        return response;
    }

    public async Task<GetLearnerSelectedResponse> GetLearnerSelected(long providerId, long learnerId)
    {
        var request = new GetLearnerSelectedRequest(providerId, learnerId);

        var response = await _outerApiClient.Get<GetLearnerSelectedResponse>(request);

        return response;
    }

    public async Task<bool> CanAccessCohort(long providerId, long cohortId)
    {
        var content = new GetCohortAccessRequest(Party.Provider, providerId, cohortId);

        var response = await _outerApiClient.Get<GetCohortAccessResponse>(content);

        return response.HasCohortAccess;
    }

    public static async Task<string> ReadFormFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        using var reader = new StreamReader(file.OpenReadStream());
        return await reader.ReadToEndAsync();
    }

    protected ApimUserInfo GetUserInfo()
    {
        if (_authenticationService.IsUserAuthenticated())
        {
            return new ApimUserInfo
            {
                UserId = _authenticationService.UserId,
                UserDisplayName = _authenticationService.UserName,
                UserEmail = _authenticationService.UserEmail
            };
        }

        return null;
    }

    public async Task<GetRplRequirementsResponse> GetRplRequirements(long providerId, long cohortId, long draftApprenticeshipId, string courseCode)
    {
        var request = new GetRplRequirementsRequest(providerId, cohortId, draftApprenticeshipId, courseCode);
        return await _outerApiClient.Get<GetRplRequirementsResponse>(request);
    }
}
