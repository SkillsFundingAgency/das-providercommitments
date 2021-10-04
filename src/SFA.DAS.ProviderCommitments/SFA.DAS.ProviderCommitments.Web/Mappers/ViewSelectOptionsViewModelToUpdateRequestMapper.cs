using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewSelectOptionsViewModelToUpdateRequestMapper : IMapper<ViewSelectOptionsViewModel, UpdateDraftApprenticeshipRequest>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ViewSelectOptionsViewModelToUpdateRequestMapper (ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }
        public async Task<UpdateDraftApprenticeshipRequest> Map(ViewSelectOptionsViewModel source)
        {
            var apiResponse = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            
            return new UpdateDraftApprenticeshipRequest
            {
                ReservationId = apiResponse.ReservationId,
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                Email = apiResponse.Email,
                DateOfBirth = apiResponse.DateOfBirth?.Date,
                Uln = apiResponse.Uln,
                CourseCode = apiResponse.CourseCode,
                Cost = apiResponse.Cost,
                StartDate = apiResponse.StartDate?.Date,
                EndDate = apiResponse.EndDate?.Date,
                Reference = apiResponse.Reference,
                CourseOption = source.SelectedOption == "-1" ? string.Empty : source.SelectedOption
            };
        }
    }
}