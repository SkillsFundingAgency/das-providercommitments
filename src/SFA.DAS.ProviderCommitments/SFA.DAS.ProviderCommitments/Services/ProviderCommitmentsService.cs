using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Models.ApiModels;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class ProviderCommitmentsService : IProviderCommitmentsService
    {
        private readonly ICommitmentsApiClient _client;
        private readonly IEncodingService _hashingService;

        public ProviderCommitmentsService(ICommitmentsApiClient client, IEncodingService hashingService)
        {
            _client = client;
            _hashingService = hashingService;
        }
        public async Task<CohortDetails> GetCohortDetail(long cohortId)
        {
            var result = await _client.GetCohort(cohortId, CancellationToken.None);
            
            return new CohortDetails
            {
                CohortId = result.CohortId, HashedCohortId = _hashingService.Encode(result.CohortId, EncodingType.CohortReference),
                AccountLegalEntityId = result.AccountLegalEntityId,
                HashedAccountLegalEntityId = _hashingService.Encode(result.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                LegalEntityName = result.LegalEntityName
            };
        }

        public Task AddDraftApprenticeshipToCohort(AddDraftApprenticeshipToCohortRequest request)
        {
            // TODO Call API Endpoint
            //return _client.AddDraftApprenticeshipToCohort(request);
            return Task.CompletedTask;
        }
    }
}
