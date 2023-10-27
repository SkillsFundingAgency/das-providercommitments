using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _outerApiClient;

        public AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper(IEncodingService encodingService, IOuterApiClient outerApiClient)
        {
            _encodingService = encodingService;
            _outerApiClient = outerApiClient;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, source.CohortId.Value, source.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            return new AddDraftApprenticeshipViewModel
            {
                AccountLegalEntityId = apiResponse.AccountLegalEntityId,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(apiResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CohortId = source.CohortId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentsPilot
            };
        }
    }
}