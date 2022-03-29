using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi
{
    public class OuterApiService : IOuterApiService
    {
        private IOuterApiClient _outerApiClient;

        public OuterApiService(IOuterApiClient outerApiClient)
        {
            _outerApiClient = outerApiClient;
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

        public async Task ValidateBulkUploadRequest(BulkUploadValidateApiRequest data)
        {
            await _outerApiClient.Post<object>(new PostValidateBulkUploadDataRequest(data));
        }
    }
}
