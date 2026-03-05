using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequestToViewModelMapper(IOuterApiClient outerApiClient)
: IMapper<DraftApprenticeshipRequest, DraftApprenticeshipSetReferenceViewModel>
{
    public async Task<DraftApprenticeshipSetReferenceViewModel> Map(DraftApprenticeshipRequest source)
    {
        var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, null);
        var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);
        var result = new DraftApprenticeshipSetReferenceViewModel
        {
            Reference = apiResponse.ProviderReference,
            OriginalReference = apiResponse.ProviderReference,
            CohortId = source.CohortId,
            CohortReference = source.CohortReference,
            DraftApprenticeshipId = source.DraftApprenticeshipId,
            Name = $"{apiResponse.FirstName} {apiResponse.LastName}",
            ProviderId = source.ProviderId,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
        };

        return result;
    }
}
