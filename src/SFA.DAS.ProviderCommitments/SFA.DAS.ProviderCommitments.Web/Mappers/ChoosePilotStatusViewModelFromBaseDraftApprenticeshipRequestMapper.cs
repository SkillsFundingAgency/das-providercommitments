using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class ChoosePilotStatusViewModelFromBaseDraftApprenticeshipRequestMapper : IMapper<BaseDraftApprenticeshipRequest, ChoosePilotStatusViewModel>
{
    public Task<ChoosePilotStatusViewModel> Map(BaseDraftApprenticeshipRequest source)
    {
        return Task.FromResult(new ChoosePilotStatusViewModel
        {
            ProviderId = source.ProviderId,
            CohortReference = source.CohortReference,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId
        });
    }
}