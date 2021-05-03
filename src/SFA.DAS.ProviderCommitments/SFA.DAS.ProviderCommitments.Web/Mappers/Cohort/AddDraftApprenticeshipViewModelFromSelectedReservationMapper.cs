using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelFromSelectedReservationMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IAuthorizationService _authorizationService;

        public AddDraftApprenticeshipViewModelFromSelectedReservationMapper(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            return new AddDraftApprenticeshipViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CohortId = source.CohortId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId,
                CourseCode = source.CourseCode,
                ShowEmail = await _authorizationService.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail)
            };
        }
    }
}