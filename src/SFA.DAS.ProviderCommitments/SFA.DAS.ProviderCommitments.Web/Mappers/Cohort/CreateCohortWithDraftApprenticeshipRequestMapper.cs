using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestMapper : IMapper<AddDraftApprenticeshipViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        public Task<CreateCohortWithDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            return Task.FromResult(new CreateCohortWithDraftApprenticeshipRequest
            {
                CourseCode = source.CourseCode,
                AccountLegalEntityId = source.AccountLegalEntityId,
                DeliveryModel = source.DeliveryModel,
                ReservationId = source.ReservationId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartMonthYear = source.StartDate.MonthYear
            });
        }
    }
}