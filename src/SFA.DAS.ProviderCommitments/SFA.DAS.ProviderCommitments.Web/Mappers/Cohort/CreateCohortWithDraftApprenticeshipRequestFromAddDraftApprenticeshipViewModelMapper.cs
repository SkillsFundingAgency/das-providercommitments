using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModel : IMapper<AddDraftApprenticeshipViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        public Task<CreateCohortWithDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            return Task.FromResult(new CreateCohortWithDraftApprenticeshipRequest
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                CourseCode = source.CourseCode,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartDate.MonthYear,
                DeliveryModel = source.DeliveryModel,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            });
        }
    }
}