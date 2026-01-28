using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class ReservationsAddDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModelMapper : IMapper<AddDraftApprenticeshipViewModel, ReservationsAddDraftApprenticeshipRequest>
{
    public Task<ReservationsAddDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
    {
        return Task.FromResult(new ReservationsAddDraftApprenticeshipRequest
        {
            ProviderId = source.ProviderId,
            CourseCode = source.CourseCode,
            DeliveryModel = source.DeliveryModel,
            ReservationId = source.ReservationId,
            StartMonthYear = source.StartDate.MonthYear,
            CohortId = source.CohortId,
            CohortReference = source.CohortReference,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            UseLearnerData = false
        });
    }
}
