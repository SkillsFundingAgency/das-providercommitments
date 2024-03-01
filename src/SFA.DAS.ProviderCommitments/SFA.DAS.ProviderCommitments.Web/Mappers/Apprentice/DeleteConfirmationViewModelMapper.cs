using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Web.Exceptions;

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
            long apprenticeshipId = 0;
            try
            {                
                apprenticeshipId = _encodingService.Decode(source.DraftApprenticeshipHashedId, EncodingType.ApprenticeshipId);                
                var commitmentId = _encodingService.Decode(source.CohortReference, EncodingType.CohortReference);
                var draftApprenticeshipResponse = await _commitmentApiClient.GetDraftApprenticeship(commitmentId, apprenticeshipId, CancellationToken.None);

                return new DeleteConfirmationViewModel
                {
                    ProviderId = source.ProviderId,
                    CohortReference = source.CohortReference,
                    DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                    ApprenticeshipName = $"{draftApprenticeshipResponse.FirstName} {draftApprenticeshipResponse.LastName}",
                    DateOfBirth = draftApprenticeshipResponse.DateOfBirth
                };
            }
            catch (RestHttpClientException restEx)
            {
                _logger.LogError(restEx, $"Error mapping apprenticeship {source.DraftApprenticeshipHashedId} to DeleteConfirmationViewModel");

                if (restEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new DraftApprenticeshipNotFoundException(
                        $"DraftApprenticeship Id: {apprenticeshipId} not found", restEx);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error mapping apprenticeship {source.DraftApprenticeshipHashedId} to DeleteConfirmationViewModel");
                throw;
            }
        }
    }
}
