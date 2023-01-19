using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortWithDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelViewModel, CreateCohortWithDraftApprenticeshipRequest>
    {
        public Task<CreateCohortWithDraftApprenticeshipRequest> Map(SelectDeliveryModelViewModel source)
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