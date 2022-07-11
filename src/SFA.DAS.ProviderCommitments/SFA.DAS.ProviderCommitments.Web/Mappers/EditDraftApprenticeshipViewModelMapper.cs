using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipViewModelMapper : IMapper<EditDraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;

        public EditDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IAuthorizationService authorizationService, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        public async Task<IDraftApprenticeshipViewModel> Map(EditDraftApprenticeshipRequest source)
        {
            try
            {
                var apiResponse = await _commitmentsApiClient.GetDraftApprenticeship(source.Request.CohortId, source.Request.DraftApprenticeshipId);

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
                    Reference = apiResponse.Reference,
                    IsContinuation = apiResponse.IsContinuation,
                    TrainingCourseOption = apiResponse.TrainingCourseOption == string.Empty ? "-1" : apiResponse.TrainingCourseOption,
                    DeliveryModel = apiResponse.DeliveryModel,
                    EmploymentPrice = apiResponse.EmploymentPrice,
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
