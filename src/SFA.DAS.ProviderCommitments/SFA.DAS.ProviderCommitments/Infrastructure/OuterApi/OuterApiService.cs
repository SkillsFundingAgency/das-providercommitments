using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi
{
    public class OuterApiService : IOuterApiService
    {
        private IOuterApiClient _outerApiClient;

        public OuterApiService(IOuterApiClient outerApiClient)
        {
            _outerApiClient = outerApiClient;
        }

        public async Task<BulkUploadAddAndApproveDraftApprenticeshipsResult> BulkUploadAddAndApproveDraftApprenticeships(BulkUploadAddAndApproveDraftApprenticeshipsRequest data)
        {
           return await _outerApiClient.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(new PostBulkUploadAddAndApproveDraftApprenticeshipsRequest(data));
        }

        public async Task<GetBulkUploadAddDraftApprenticeshipsResult> BulkUploadDraftApprenticeships(BulkUploadAddDraftApprenticeshipsRequest data)
        {
            return await _outerApiClient.Post<GetBulkUploadAddDraftApprenticeshipsResult>(new PostBulkUploadAddDraftApprenticeshipsRequest(data));
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

        public async Task<GetAllCohortDetailsQueryResult> GetAllCohortDetails(long providerId, long cohortId)
        {
            return await _outerApiClient.Get<GetAllCohortDetailsQueryResult>(new GetAllCohortDetailsQueryRequest(providerId, cohortId));
        }
    }
}