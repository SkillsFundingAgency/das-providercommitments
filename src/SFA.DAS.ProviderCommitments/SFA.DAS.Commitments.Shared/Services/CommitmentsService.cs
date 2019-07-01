using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;

namespace SFA.DAS.Commitments.Shared.Services
{
    public class CommitmentsService : ICommitmentsService
    {
        private readonly ICommitmentsApiClient _client;
        private readonly IEncodingService _hashingService;

        public CommitmentsService(ICommitmentsApiClient client, IEncodingService hashingService)
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
                LegalEntityName = result.LegalEntityName,
                IsFundedByTransfer = result.IsFundedByTransfer
            };
        }

        public Task AddDraftApprenticeshipToCohort(long cohortId, AddDraftApprenticeshipRequest request)
        {
            return _client.AddDraftApprenticeship(cohortId, request);
        }

        public Task<CreateCohortResponse> CreateCohort(CreateCohortRequest request)
        {
            return _client.CreateCohort(request, CancellationToken.None);
        }

        public async Task<EditDraftApprenticeshipDetails> GetDraftApprenticeshipForCohort(int providerId, long cohortId, long draftApprenticeshipId)
        {
            var result = await _client.GetDraftApprenticeship(cohortId, draftApprenticeshipId);

            return new EditDraftApprenticeshipDetails
            {
                DraftApprenticeshipId = result.Id,
                DraftApprenticeshipHashedId = _hashingService.Encode(result.Id, EncodingType.ApprenticeshipId),
                CohortId = cohortId,
                ProviderId = providerId,
                CohortReference = _hashingService.Encode(cohortId, EncodingType.CohortReference),
                ReservationId = result.ReservationId,
                FirstName = result.FirstName,
                LastName = result.LastName,
                DateOfBirth = result.DateOfBirth,
                UniqueLearnerNumber = result.Uln,
                CourseCode = result.CourseCode,
                Cost = result.Cost,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                OriginatorReference = result.Reference
            };
        }

        public Task UpdateDraftApprenticeship(long cohortId, long draftApprenticeshipId, UpdateDraftApprenticeshipRequest updateRequest)
        {
            return _client.UpdateDraftApprenticeship(cohortId, draftApprenticeshipId, updateRequest);
        }
    }
}