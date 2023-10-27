using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class ChoosePilotStatusViewModelFromBaseDraftApprenticeshipRequestMapper : IMapper<BaseDraftApprenticeshipRequest, ChoosePilotStatusViewModel>
{
    public Task<ChoosePilotStatusViewModel> Map(BaseDraftApprenticeshipRequest source)
    {
        return Task.FromResult(new ChoosePilotStatusViewModel
        {
            CacheKey = source.CacheKey,
            ProviderId = source.ProviderId,
            CohortReference = source.CohortReference,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId
        });
    }
}