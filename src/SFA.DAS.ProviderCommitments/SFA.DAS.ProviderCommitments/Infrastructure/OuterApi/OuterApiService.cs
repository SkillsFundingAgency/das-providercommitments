using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public class OuterApiService(IOuterApiClient outerApiClient, IAuthenticationServiceForApim authenticationService)
    : IOuterApiService
{
    public async Task<BulkUploadAddAndApproveDraftApprenticeshipsResult> BulkUploadAddAndApproveDraftApprenticeships(BulkUploadAddAndApproveDraftApprenticeshipsRequest data)
    {
        try
        {
            return await outerApiClient.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(new PostBulkUploadAddAndApproveDraftApprenticeshipsRequest(data));
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
            return await outerApiClient.Post<GetBulkUploadAddDraftApprenticeshipsResult>(
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
        return await outerApiClient.Get<GetAccountLegalEntityQueryResult>(new GetAccountLegalEntityRequest(publicAccountLegalEntityId));
    }

    public async Task<GetCohortResult> GetCohort(long cohortId)
    {
        return await outerApiClient.Get<GetCohortResult>(new GetCohortRequest(cohortId));
    }

    public async Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId)
    {
        return await outerApiClient.Get<GetDraftApprenticeshipsResult>(new GetDraftApprenticeshipsRequest(cohortId));
    }

    public async Task<GetStandardResponse> GetStandardDetails(string courseCode)
    {
        return await outerApiClient.Get<GetStandardResponse>(new GetStandardDetailsRequest(courseCode));
    }

    public async Task ValidateBulkUploadRequest(BulkUploadValidateApimRequest data)
    {
        await outerApiClient.Post<object>(new PostValidateBulkUploadDataRequest(data));
    }

    public async Task CreateOverlappingTrainingDateRequest(CreateOverlappingTrainingDateApimRequest data)
    {
        await outerApiClient.Post<CreateOverlappingTrainingDateResponse>(new PostCreateOveralappingTrainingDateRequest(data));
    }

    public async Task ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(ValidateDraftApprenticeshipApimRequest data)
    {
        await outerApiClient.Post<object>(new PostValidateDraftApprenticeshipforOverlappingTrainingDateRequest(data));
    }

    public async Task<ValidateUlnOverlapOnStartDateQueryResult> ValidateUlnOverlapOnStartDate(long providerId, string uln, string startDate, string endDate)
    {
        return await outerApiClient.Get<ValidateUlnOverlapOnStartDateQueryResult>(new ValidateUlnOverlapOnStartDateQueryRequest(providerId, uln, startDate, endDate));
    }

    public async Task ValidateChangeOfEmployerOverlap(ValidateChangeOfEmployerOverlapApimRequest data)
    {
        await outerApiClient.Post<object>(new PostValidateChangeOfEmployerOverlapRequest(data));
    }

    public async Task<GetOverlapRequestQueryResult> GetOverlapRequest(long apprenticeshipId)
    {
        return await outerApiClient.Get<GetOverlapRequestQueryResult>(new GetOverlapRequestQueryRequest(apprenticeshipId));
    }

    public async Task UpdateDraftApprenticeship(long cohortId, long apprenticeshipId, UpdateDraftApprenticeshipApimRequest request)
    {
        await outerApiClient.Put<object>(new PutUpdateDraftApprenticeshipRequest(cohortId, apprenticeshipId) { Data = request });
    }

    public async Task DraftApprenticeshipAddEmail(long providerId, long cohortId, long apprenticeshipId, DraftApprenticeAddEmailApimRequest request)
    {
        await outerApiClient.Put<object>(new DraftApprenticeAddEmailRequest(providerId, cohortId, apprenticeshipId) { Data = request });
    }

    public async Task DraftApprenticeshipSetReference(long providerId, long cohortId, long apprenticeshipId, PostDraftApprenticeshipSetReferenceApimRequest request )
    {
        await outerApiClient.Put<object>(new PostDraftApprenticeshipSetReferenceRequest(providerId, cohortId, apprenticeshipId) { Data = request });
    }

    public async Task<AddDraftApprenticeshipResponse> AddDraftApprenticeship(long cohortId, AddDraftApprenticeshipApimRequest request)
    {
        return await outerApiClient.Post<AddDraftApprenticeshipResponse>(new PostAddDraftApprenticeshipRequest(cohortId) { Data = request });
    }

    public async Task<CreateCohortResponse> CreateCohort(CreateCohortApimRequest request)
    {
        return await outerApiClient.Post<CreateCohortResponse>(new PostCreateCohortRequest { Data = request });
    }

    public async Task<GetPriorLearningDataQueryResult> GetPriorLearningData(long providerId, long cohortId, long draftApprenticeshipId)
    {
        return await outerApiClient.Get<GetPriorLearningDataQueryResult>(new GetPriorLearningDataQueryRequest(providerId, cohortId, draftApprenticeshipId));
    }

    public async Task<CreatePriorLearningDataResponse> UpdatePriorLearningData(long providerId, long cohortId, long draftApprenticeshipId, CreatePriorLearningDataRequest request)
    {
        return await outerApiClient.Post<CreatePriorLearningDataResponse>(new PostPriorLearningDataRequest(providerId, cohortId, draftApprenticeshipId) { Data = request });
    }

    public async Task<GetPriorLearningSummaryQueryResult> GetPriorLearningSummary(long providerId, long cohortId, long draftApprenticeshipId)
    {
        return await outerApiClient.Get<GetPriorLearningSummaryQueryResult>(new GetPriorLearningSummaryQueryRequest(providerId, cohortId, draftApprenticeshipId));
    }

    public async Task<GetCohortDetailsResponse> GetCohortDetails(long providerId, long cohortId)
    {
        return await outerApiClient.Get<GetCohortDetailsResponse>(new GetCohortDetailsRequest(providerId, cohortId));
    }

    public async Task<PostApprenticeshipsCSVResponse> GetApprenticeshipsCSV(PostApprenticeshipsCSVRequest request)
    {
      return await outerApiClient.Post<PostApprenticeshipsCSVResponse>(request);
    }

    // <inherit-doc />
    public async Task<ProviderAccountResponse> GetProviderStatus(long ukprn)
    {
        return await outerApiClient.Get<ProviderAccountResponse>(new GetProviderStatusDetails(ukprn));
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

        var response = await outerApiClient.Post<FileUploadLogResponse>(new PostFileUploadLogRequest(request));
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

        await outerApiClient.Put<object>(new PutFileUploadUpdateLogRequest(fileUploadLogId, content));
    }

    public async Task AddUnhandledExceptionToFileUploadLog(long providerId, long fileUploadLogId, string errorMessage)
    {
        var content = new FileUploadUpdateLogWithErrorContentRequest
        {
            ProviderId = providerId,
            ErrorContent = "Unhandled exception \r\n" + errorMessage,
            UserInfo = GetUserInfo()
        };

        await outerApiClient.Put<object>(new PutFileUploadUpdateLogRequest(fileUploadLogId, content));
    }

    public async Task<bool> HasPermission(long ukprn, long? accountLegalEntityId)
    {
        var content = new GetHasPermissionRequest(ukprn, accountLegalEntityId.GetValueOrDefault());
        var response = await outerApiClient.Get<GetHasPermissionResponse>(content);
        return response.HasPermission;
    }

    public async Task<bool> HasRelationshipWithPermission(long? ukprn)
    {
        var content = new GetHasRelationshipWithPermissionRequest(ukprn);

        var response = await outerApiClient.Get<GetHasPermissionResponse>(content);

        return response.HasPermission;
    }

    public async Task<bool> CanAccessApprenticeship(long providerId, long apprenticeshipId)
    {
        var content = new GetApprenticeshipAccessRequest(Party.Provider, providerId, apprenticeshipId);

        var response = await outerApiClient.Get<GetApprenticeshipAccessResponse>(content);

        return response.HasApprenticeshipAccess;
    }

    public async Task<GetLearnerDetailsForProviderResponse> GetLearnerDetailsForProvider(long providerId, long? accountLegalEntityId, long? cohortId, string searchTerm, string sortColumn, bool sortDesc, int page, int? startMonth, int startYear)
    {
        var request = new GetLearnerDetailsForProviderRequest(providerId, accountLegalEntityId, cohortId, searchTerm, sortColumn, sortDesc, page, startMonth, startYear);

        var response = await outerApiClient.Get<GetLearnerDetailsForProviderResponse>(request);

        return response;
    }

    public async Task<GetLearnerSelectedResponse> GetLearnerSelected(long providerId, long learnerId)
    {
        var request = new GetLearnerSelectedRequest(providerId, learnerId);

        var response = await outerApiClient.Get<GetLearnerSelectedResponse>(request);

        return response;
    }

    public async Task<bool> CanAccessCohort(long providerId, long cohortId)
    {
        var content = new GetCohortAccessRequest(Party.Provider, providerId, cohortId);

        var response = await outerApiClient.Get<GetCohortAccessResponse>(content);

        return response.HasCohortAccess;
    }

    public async Task<SyncLearnerDataResponse> SyncLearnerData(long providerId, long cohortId, long draftApprenticeshipId)
    {
        var request = new SyncLearnerDataRequest(providerId, cohortId, draftApprenticeshipId);

        var response = await outerApiClient.Post<SyncLearnerDataResponse>(request);

        return response;
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
        if (authenticationService.IsUserAuthenticated())
        {
            return new ApimUserInfo
            {
                UserId = authenticationService.UserId,
                UserDisplayName = authenticationService.UserName,
                UserEmail = authenticationService.UserEmail
            };
        }

        return null;
    }

    public async Task<GetRplRequirementsResponse> GetRplRequirements(long providerId, long cohortId, long draftApprenticeshipId, string courseCode)
    {
        var request = new GetRplRequirementsRequest(providerId, cohortId, draftApprenticeshipId, courseCode);
        return await outerApiClient.Get<GetRplRequirementsResponse>(request);
    }

    public async Task<ValidateEditApprenticeshipResponse> EditApprenticeship(long providerId, long apprenticeshipId, ValidateEditApprenticeshipRequest request)
    {
        var apiRequest = new PutEditApprenticeshipRequest(providerId, apprenticeshipId)
        {
            Data = request
        };
        
        return await outerApiClient.Put<ValidateEditApprenticeshipResponse>(apiRequest);
    }

    public async Task<ConfirmEditApprenticeshipResponse> ConfirmEditApprenticeship(long providerId, long apprenticeshipId, ConfirmEditApprenticeshipRequest request)
    {
        var apiRequest = new PostConfirmEditApprenticeshipRequest(providerId, apprenticeshipId, request);
        
        return await outerApiClient.Post<ConfirmEditApprenticeshipResponse>(apiRequest);
    }
}
