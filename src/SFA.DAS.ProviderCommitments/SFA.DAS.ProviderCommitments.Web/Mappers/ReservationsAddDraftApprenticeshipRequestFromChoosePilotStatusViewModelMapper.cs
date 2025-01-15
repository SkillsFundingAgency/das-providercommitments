using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class ReservationsAddDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper : IMapper<ChoosePilotStatusViewModel, ReservationsAddDraftApprenticeshipRequest>
{
    public Task<ReservationsAddDraftApprenticeshipRequest> Map(ChoosePilotStatusViewModel source)
    {
        return Task.FromResult(new ReservationsAddDraftApprenticeshipRequest
        {
            ProviderId = source.ProviderId,
            CohortReference = source.CohortReference,
            CourseCode = source.CourseCode,
            DeliveryModel = source.DeliveryModel,
            ReservationId = source.ReservationId.Value,
            StartMonthYear = source.StartMonthYear,
            IsOnFlexiPaymentPilot = source.Selection == null ? null : source.Selection.Value == ChoosePilotStatusOptions.Pilot
        });
    }
}