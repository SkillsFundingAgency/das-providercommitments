using System;
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
                CohortId = result.CohortId,
                HashedCohortId = _hashingService.Encode(result.CohortId, EncodingType.CohortReference),
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

        public Task<EditDraftApprenticeshipDetails> GetDraftApprenticeshipForCohort(long cohortId, long draftApprenticeshipId)
        {
            // TODO Call API Endpoint
            //var _client.GetDraftApprenticeship(cohortId, draftApprenticeshipId);
            // Map to EditDraftApprenticeshipDetails
            return Task.FromResult(new EditDraftApprenticeshipDetails
            {
                DraftApprenticeshipId = draftApprenticeshipId,
                DraftApprenticeshipHashedId =
                    _hashingService.Encode(draftApprenticeshipId, EncodingType.ApprenticeshipId),
                CohortId = cohortId,
                CohortReference = _hashingService.Encode(cohortId, EncodingType.CohortReference),
                LegalEntityName = "LEN For Editing",
                ReservationId = null,
                FirstName = "First",
                LastName = "Last",
                DateOfBirth = DateTime.Today.AddYears(-30),
                UniqueLearnerNumber = "01234567899",
                CourseCode = "174",
                Cost = 1000,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                OriginatorReference = null
            });
        }
    }
}