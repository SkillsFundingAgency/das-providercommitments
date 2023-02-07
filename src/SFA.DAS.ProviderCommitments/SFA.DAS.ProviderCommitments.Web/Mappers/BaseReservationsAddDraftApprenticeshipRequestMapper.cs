using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class BaseReservationsAddDraftApprenticeshipRequestMapper : IMapper<AddDraftApprenticeshipViewModel, BaseReservationsAddDraftApprenticeshipRequest>
    {
        public Task<BaseReservationsAddDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            return Task.FromResult(new BaseReservationsAddDraftApprenticeshipRequest
            {
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartDate.MonthYear,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentPilot
            });
        }
    }
}