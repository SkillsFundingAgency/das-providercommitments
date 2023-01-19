using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class CreateCohortWithDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper : IMapper<ChoosePilotStatusViewModel, CreateCohortWithDraftApprenticeshipRequest>
{
    public Task<CreateCohortWithDraftApprenticeshipRequest> Map(ChoosePilotStatusViewModel source)
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
            ShowTrainingDetails = source.ShowTrainingDetails,
            IsOnFlexiPaymentPilot = source.Selection == ChoosePilotStatusOptions.Pilot
        });
    }
}