using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class ChoosePilotStatusViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, ChoosePilotStatusViewModel>
{
    public Task<ChoosePilotStatusViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
    {
        return Task.FromResult(new ChoosePilotStatusViewModel
        {
            Selection = !source.IsOnFlexiPaymentPilot.HasValue ? null : source.IsOnFlexiPaymentPilot.Value ? ChoosePilotStatusOptions.Pilot : ChoosePilotStatusOptions.NonPilot
        });
    }
}