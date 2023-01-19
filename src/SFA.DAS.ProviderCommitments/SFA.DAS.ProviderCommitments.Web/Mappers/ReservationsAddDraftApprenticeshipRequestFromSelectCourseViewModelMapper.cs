using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ReservationsAddDraftApprenticeshipRequestFromSelectCourseViewModelMapper : IMapper<SelectCourseViewModel, ReservationsAddDraftApprenticeshipRequest>
    {
        public Task<ReservationsAddDraftApprenticeshipRequest> Map(SelectCourseViewModel source)
        {
            return Task.FromResult(new ReservationsAddDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartMonthYear
            });
        }
    }
}