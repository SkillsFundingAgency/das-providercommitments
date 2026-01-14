using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipViewModelSetReferenceApimRequestMapper
: IMapper<DraftApprenticeshipSetReferenceViewModel, DraftApprenticeshipSetReferenceApimRequest>
{
    public async Task<DraftApprenticeshipSetReferenceApimRequest> Map(DraftApprenticeshipSetReferenceViewModel source)
    {
        var result = new DraftApprenticeshipSetReferenceApimRequest()
        {
            Reference = source.Reference,
            Party = source.Party,
        };

        return result;
    }
}
