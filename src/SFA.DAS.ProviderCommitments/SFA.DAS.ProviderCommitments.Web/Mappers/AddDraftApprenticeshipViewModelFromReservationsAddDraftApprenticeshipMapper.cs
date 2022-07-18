using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _apiClient;
        private readonly IEncodingService _encodingService;

        public AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper(ICommitmentsApiClient apiClient, IEncodingService encodingService)
        {
            _apiClient = apiClient;
            _encodingService = encodingService;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            var cohort = await _apiClient.GetCohort(source.CohortId.Value);

            return new AddDraftApprenticeshipViewModel
            {
                AccountLegalEntityId = cohort.AccountLegalEntityId,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(cohort.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CohortId = source.CohortId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel
            };
        }
    }
}