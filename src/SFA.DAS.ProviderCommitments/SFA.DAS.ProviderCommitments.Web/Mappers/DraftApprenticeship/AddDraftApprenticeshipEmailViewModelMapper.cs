using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
public class AddDraftApprenticeshipEmailViewModelMapper(IOuterApiClient outerApiClient)
: IMapper<DraftApprenticeshipAddEmailRequest, DraftApprenticeshipAddEmailViewModel>
{
    public async Task<DraftApprenticeshipAddEmailViewModel> Map(DraftApprenticeshipAddEmailRequest source)
    {

        var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, (long)source.CohortId, (long)source.DraftApprenticeshipId, null);
        var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);

        var result = new DraftApprenticeshipAddEmailViewModel()
        {
            ProviderId = source.ProviderId,
            CohortId = source.CohortId,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
            Name = $"{apiResponse.FirstName} {apiResponse.LastName}",
            Email = apiResponse.Email,
            OriginalEmail = apiResponse.Email,
            CohortReference = source.CohortReference,
            DraftApprenticeshipId = source.DraftApprenticeshipId,
        };

        return result;
    }
}
