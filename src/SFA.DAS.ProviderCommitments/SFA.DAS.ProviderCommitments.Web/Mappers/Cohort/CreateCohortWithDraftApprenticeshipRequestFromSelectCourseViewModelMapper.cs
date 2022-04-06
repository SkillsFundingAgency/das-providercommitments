using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper : IMapper<SelectCourseViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        public Task<CreateCohortWithDraftApprenticeshipRequest> Map(SelectCourseViewModel source)
        {
            return Task.FromResult(new CreateCohortWithDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                CourseCode = source.CourseCode,
                StartMonthYear = source.StartMonthYear,
                DeliveryModel = source.DeliveryModel
            });
        }
    }
}