using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipViewModelMapper : IMapper<EditDraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _outerApiClient;

        public EditDraftApprenticeshipViewModelMapper(IEncodingService encodingService, IOuterApiClient outerApiClient)
        {
            _encodingService = encodingService;
            _outerApiClient = outerApiClient;
        }

        public async Task<IDraftApprenticeshipViewModel> Map(EditDraftApprenticeshipRequest source)
        {
            try
            {
                var apiRequest = new GetEditDraftApprenticeshipRequest(source.Request.ProviderId, source.Request.CohortId, source.Request.DraftApprenticeshipId);
                var apiResponse = await _outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);

                return new EditDraftApprenticeshipViewModel(apiResponse.DateOfBirth, apiResponse.StartDate, apiResponse.EndDate, apiResponse.EmploymentEndDate)
                {
                    AccountLegalEntityId = source.Cohort.AccountLegalEntityId,
                    EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(source.Cohort.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
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
                    HasStandardOptions = apiResponse.HasStandardOptions,
                    Cost = apiResponse.Cost,
                    Reference = apiResponse.ProviderReference,
                    IsContinuation = apiResponse.IsContinuation,
                    TrainingCourseOption = apiResponse.TrainingCourseOption == string.Empty ? "-1" : apiResponse.TrainingCourseOption,
                    DeliveryModel = (DeliveryModel?) apiResponse.DeliveryModel,
                    EmploymentPrice = apiResponse.EmploymentPrice,
                    RecognisePriorLearning = apiResponse.RecognisePriorLearning,
                    DurationReducedBy = apiResponse.DurationReducedBy,
                    PriceReducedBy = apiResponse.PriceReducedBy,
                    RecognisingPriorLearningStillNeedsToBeConsidered = apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered,
                    HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                    HasUnavailableFlexiJobAgencyDeliveryModel = apiResponse.HasUnavailableDeliveryModel && apiResponse.DeliveryModel == Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency
                };
            }
            catch (RestHttpClientException restEx)
            {
                if (restEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new DraftApprenticeshipNotFoundException(
                        $"DraftApprenticeship Id: {source.Request.DraftApprenticeshipId} not found", restEx);
                }
                throw;
            }
        }
    }
}
