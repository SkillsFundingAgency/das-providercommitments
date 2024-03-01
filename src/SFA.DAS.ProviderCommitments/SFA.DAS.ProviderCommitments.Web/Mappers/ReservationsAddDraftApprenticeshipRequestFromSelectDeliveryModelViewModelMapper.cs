using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ReservationsAddDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelViewModel, ReservationsAddDraftApprenticeshipRequest>
    {
        public Task<ReservationsAddDraftApprenticeshipRequest> Map(SelectDeliveryModelViewModel source)
        {
            return Task.FromResult(new ReservationsAddDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartMonthYear,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentsPilot
            });
        }
    }
}