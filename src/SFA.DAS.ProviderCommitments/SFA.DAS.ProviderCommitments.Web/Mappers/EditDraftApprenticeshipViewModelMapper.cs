using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipViewModelMapper : IMapper<EditDraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IAuthorizationService _authorizationService;

        public EditDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IAuthorizationService authorizationService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _authorizationService = authorizationService;
        }

        public async Task<IDraftApprenticeshipViewModel> Map(EditDraftApprenticeshipRequest source)
        {
            var apiResponse = await _commitmentsApiClient.GetDraftApprenticeship(source.Request.CohortId, source.Request.DraftApprenticeshipId);

            return new EditDraftApprenticeshipViewModel(apiResponse.DateOfBirth, apiResponse.StartDate, apiResponse.EndDate)
            {
                DraftApprenticeshipId = source.Request.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = source.Request.DraftApprenticeshipHashedId,
                CohortId = source.Request.CohortId,
                CohortReference = source.Request.CohortReference, 
                ProviderId = source.Request.ProviderId,
                ReservationId = apiResponse.ReservationId,
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                Email = apiResponse.Email,
                Uln = apiResponse.Uln,
                CourseCode = apiResponse.CourseCode,
                Cost = apiResponse.Cost,
                Reference = apiResponse.Reference,
                IsContinuation = apiResponse.IsContinuation,
                ShowEmail = await _authorizationService.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail)
            };
        }
    }
}
