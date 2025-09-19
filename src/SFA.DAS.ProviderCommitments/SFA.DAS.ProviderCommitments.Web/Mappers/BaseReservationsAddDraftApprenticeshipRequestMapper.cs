using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class BaseReservationsAddDraftApprenticeshipRequestMapper : IMapper<AddDraftApprenticeshipViewModel, BaseReservationsAddDraftApprenticeshipRequest>
{
    public Task<BaseReservationsAddDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
    {
        return Task.FromResult(new BaseReservationsAddDraftApprenticeshipRequest
        {
            ProviderId = source.ProviderId,
            CourseCode = source.CourseCode,
            DeliveryModel = source.DeliveryModel,
            ReservationId = source.ReservationId,
            StartMonthYear = source.StartDate.MonthYear
        });
    }
}