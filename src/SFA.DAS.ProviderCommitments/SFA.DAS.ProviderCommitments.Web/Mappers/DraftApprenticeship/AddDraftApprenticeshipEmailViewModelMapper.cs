using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
public class AddDraftApprenticeshipEmailViewModelMapper
: IMapper<DraftApprenticeshipAddEmailRequest, DraftApprenticeshipAddEmailViewModel>
{
    public async Task<DraftApprenticeshipAddEmailViewModel> Map(DraftApprenticeshipAddEmailRequest source)
    {
        var result = new DraftApprenticeshipAddEmailViewModel()
        {
            ProviderId = source.ProviderId,
            CohortId = source.CohortId,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
            Name = source.Name,
            Email = source.Email,
            CohortReference = source.CohortReference,
            DraftApprenticeshipId = source.DraftApprenticeshipId,
            EndDate = source.EndDate,
            StartDate = source.StartDate,
        };

        return result;
    }
}
