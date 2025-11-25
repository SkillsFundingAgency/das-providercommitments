using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequestToViewModelMapper
: IMapper<DraftApprenticeshipSetReferenceRequest, DraftApprenticeshipSetReferenceViewModel>
{
    public async Task<DraftApprenticeshipSetReferenceViewModel> Map(DraftApprenticeshipSetReferenceRequest source)
    {
        var result = new DraftApprenticeshipSetReferenceViewModel
        {
            Reference = source.Reference,
            Party = source.Party,
            CohortId = source.CohortId,
            DraftApprenticeshipId = source.DraftApprenticeshipId,
            Name = source.Name,
            ProviderId = source.ProviderId,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
        };

        return result;
    }
}
