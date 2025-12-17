using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequestToViewModelMapper(IOuterApiClient outerApiClient)
: IMapper<DraftApprenticeshipSetReferenceRequest, DraftApprenticeshipSetReferenceViewModel>
{
    public async Task<DraftApprenticeshipSetReferenceViewModel> Map(DraftApprenticeshipSetReferenceRequest source)
    {
        var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, (long)source.CohortId, (long)source.DraftApprenticeshipId, null);
        var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);
        var result = new DraftApprenticeshipSetReferenceViewModel
        {
            Reference = apiResponse.ProviderReference,
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
