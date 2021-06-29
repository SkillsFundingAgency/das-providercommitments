using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DeleteConfirmationViewModelMapper : IMapper<DeleteConfirmationRequest, DeleteConfirmationViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<DeleteConfirmationViewModelMapper> _logger;

        public DeleteConfirmationViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService,
           ILogger<DeleteConfirmationViewModelMapper> logger)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _logger = logger;
        }

        public async Task<DeleteConfirmationViewModel> Map(DeleteConfirmationRequest source)
        {
            try
            {                
                var apprenticeshipId = _encodingService.Decode(source.ApprenticeshipHashedId, EncodingType.ApprenticeshipId);
                var commitmentId = _encodingService.Decode(source.CommitmentHashedId, EncodingType.CohortReference);
                var draftApprenticeshipResponse = await _commitmentApiClient.GetDraftApprenticeship(commitmentId, apprenticeshipId, CancellationToken.None);

                return new DeleteConfirmationViewModel
                {
                    ProviderId = source.ProviderId,
                    CommitmentHashedId = source.CommitmentHashedId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeshipName = $"{draftApprenticeshipResponse.FirstName} {draftApprenticeshipResponse.LastName}",
                    DateOfBirth = draftApprenticeshipResponse.DateOfBirth,
                    ApprenticeshipId = apprenticeshipId,
                    CommitmentId = commitmentId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error mapping apprenticeship {source.ApprenticeshipHashedId} to DeleteConfirmationViewModel");
                throw;
            }
        }
    }
}
