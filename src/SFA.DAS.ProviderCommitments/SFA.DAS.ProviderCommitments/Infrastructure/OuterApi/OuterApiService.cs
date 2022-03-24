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

        public async Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId)
        {
            return await _outerApiClient.Get<GetDraftApprenticeshipsResult>(new GetDraftApprenticeshipsRequest(cohortId));
        }
    }
}
