using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class BaseDraftApprenticeshipRequestFromChoosePilotStatusViewModelMapper : IMapper<ChoosePilotStatusViewModel, BaseDraftApprenticeshipRequest>
{
    public Task<BaseDraftApprenticeshipRequest> Map(ChoosePilotStatusViewModel source)
    {
        return Task.FromResult(new BaseDraftApprenticeshipRequest
        {
            ProviderId = source.ProviderId,
            CohortReference = source.CohortReference,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId
        });
    }
}