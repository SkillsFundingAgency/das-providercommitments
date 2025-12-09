using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipAddEmailViewModelToApimRequestMapper : IMapper<DraftApprenticeshipAddEmailViewModel, DraftApprenticeAddEmailApimRequest>
{
    public async Task<DraftApprenticeAddEmailApimRequest> Map(DraftApprenticeshipAddEmailViewModel source)
    {
        var result = new DraftApprenticeAddEmailApimRequest()
        {
            Email = source.Email,
            CohortId = source.CohortId,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
        };

        return result;
    }
}
