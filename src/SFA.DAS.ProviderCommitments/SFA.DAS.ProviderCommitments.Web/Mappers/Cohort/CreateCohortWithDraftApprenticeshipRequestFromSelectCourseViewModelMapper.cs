using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper : IMapper<Models.Cohort.SelectCourseViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        public Task<CreateCohortWithDraftApprenticeshipRequest> Map(Models.Cohort.SelectCourseViewModel source)
        {
            return Task.FromResult(new CreateCohortWithDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                CourseCode = source.CourseCode,
                StartMonthYear = source.StartMonthYear,
                DeliveryModel = source.DeliveryModel,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentsPilot
            });
        }
    }
}