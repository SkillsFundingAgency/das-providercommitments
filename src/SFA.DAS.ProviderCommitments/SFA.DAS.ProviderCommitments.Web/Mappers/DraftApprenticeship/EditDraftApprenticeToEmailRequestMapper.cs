using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
public class EditDraftApprenticeToEmailRequestMapper(
     IOuterApiClient outerApiClient) : IMapper<EditDraftApprenticeshipViewModel, DraftApprenticeshipAddEmailRequest>
{
    public async Task<DraftApprenticeshipAddEmailRequest> Map(EditDraftApprenticeshipViewModel source)
    {

        var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, (long)source.CohortId,(long)source.DraftApprenticeshipId,null);
        var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);       

        return new DraftApprenticeshipAddEmailRequest
        {
            Email = apiResponse.Email,
            CohortId = source.CohortId.GetValueOrDefault(),
            CohortReference = source.CohortReference,
            ProviderId = source.ProviderId,
            DraftApprenticeshipId = source.DraftApprenticeshipId.Value,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
            Name = $"{apiResponse.FirstName} {apiResponse.LastName}",
            StartDate = apiResponse.StartDate.Value.ToShortDateString(),
            EndDate = apiResponse.EndDate.Value.ToShortDateString(),
        };
    }
}
