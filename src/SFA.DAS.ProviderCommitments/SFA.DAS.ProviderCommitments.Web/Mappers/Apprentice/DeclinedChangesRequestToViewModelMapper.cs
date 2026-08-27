using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class DeclinedChangesRequestToViewModelMapper(IOuterApiClient outerApiClient)
    : IMapper<InvalidIlrChangesRequest, DeclinedChangesViewModel>
{
    public async Task<DeclinedChangesViewModel> Map(InvalidIlrChangesRequest source)
    {
        var response = await outerApiClient.Get<GetInvalidIlrChangesResponse>(
            new GetInvalidIlrChangesRequest(
                source.ProviderId,
                source.ApprenticeshipId,
                GetInvalidIlrChangesRequest.DeclinedChangesPath));

        return InvalidIlrChangesRequestToViewModelMapper.MapResponse(
            source,
            response,
            new DeclinedChangesViewModel(),
            InvalidIlrChangesRequestToViewModelMapper.ApplyDeclinedCopy);
    }
}
