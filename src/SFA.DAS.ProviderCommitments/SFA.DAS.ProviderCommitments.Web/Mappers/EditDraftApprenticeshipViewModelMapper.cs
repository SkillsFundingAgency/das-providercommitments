using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipViewModelMapper : IMapper<EditDraftApprenticeshipRequest, EditDraftApprenticeshipViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public EditDraftApprenticeshipViewModelMapper(IEncodingService encodingService, ICommitmentsApiClient commitmentsApiClient)
        {
            _encodingService = encodingService;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<EditDraftApprenticeshipViewModel> Map(EditDraftApprenticeshipRequest source)
        {
            var apiResponse = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId.Value, source.DraftApprenticeshipId.Value);

            return new EditDraftApprenticeshipViewModel(apiResponse.DateOfBirth, apiResponse.StartDate, apiResponse.EndDate)
            {
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = _encodingService.Encode(apiResponse.Id, EncodingType.ApprenticeshipId),
                CohortId = source.CohortId.Value,
                CohortReference = _encodingService.Encode(source.CohortId.Value, EncodingType.CohortReference),
                ProviderId = source.ProviderId,
                ReservationId = apiResponse.ReservationId,
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                Uln = apiResponse.Uln,
                CourseCode = apiResponse.CourseCode,
                Cost = apiResponse.Cost,
                Reference = apiResponse.Reference,
                IsContinuation = apiResponse.IsContinuation
            };
        }
    }
}
