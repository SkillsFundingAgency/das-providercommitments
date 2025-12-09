using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipViewModelSetRefrenceApimRequestMapper
: IMapper<DraftApprenticeshipSetReferenceViewModel, PostDraftApprenticeshipSetReferenceApimRequest>
{
    public async Task<PostDraftApprenticeshipSetReferenceApimRequest> Map(DraftApprenticeshipSetReferenceViewModel source)
    {
        var result = new PostDraftApprenticeshipSetReferenceApimRequest()
        {
            Reference = source.Reference,
            Party = source.Party,
        };

        return result;
    }
}
