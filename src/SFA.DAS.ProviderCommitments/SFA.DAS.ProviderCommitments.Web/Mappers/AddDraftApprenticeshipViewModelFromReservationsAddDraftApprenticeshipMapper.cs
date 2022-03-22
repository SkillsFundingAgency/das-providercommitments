using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        public Task<AddDraftApprenticeshipViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            return Task.FromResult(new AddDraftApprenticeshipViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CohortId = source.CohortId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel
            });
        }
    }
}