using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewSelectOptionsViewModelToUpdateRequestMapper(IOuterApiClient outerApiClient) : IMapper<ViewSelectOptionsViewModel, UpdateDraftApprenticeshipApimRequest>
    {       
        public async Task<UpdateDraftApprenticeshipApimRequest> Map(ViewSelectOptionsViewModel source)
        {
            var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, (long)source.CohortId, (long)source.DraftApprenticeshipId, null);
            var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);

            return new UpdateDraftApprenticeshipApimRequest
            {
                ReservationId = apiResponse.ReservationId,
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                Email = apiResponse.Email,
                DateOfBirth = apiResponse.DateOfBirth?.Date,
                Uln = apiResponse.Uln,
                CourseCode = apiResponse.CourseCode,
                Cost = apiResponse.Cost,
                TrainingPrice = apiResponse.TrainingPrice,
                EndPointAssessmentPrice = apiResponse.EndPointAssessmentPrice,
                ActualStartDate = apiResponse.ActualStartDate,
                StartDate = apiResponse.StartDate?.Date,
                EndDate = apiResponse.EndDate?.Date,
                Reference = apiResponse.ProviderReference,
                CourseOption = source.SelectedOption == "-1" ? string.Empty : source.SelectedOption,
                DeliveryModel = apiResponse.DeliveryModel,
                EmploymentEndDate = apiResponse.EmploymentEndDate,
                EmploymentPrice = apiResponse.EmploymentPrice,
                LearnerDataId = apiResponse.LearnerDataId,               
            };
        }
    }
}