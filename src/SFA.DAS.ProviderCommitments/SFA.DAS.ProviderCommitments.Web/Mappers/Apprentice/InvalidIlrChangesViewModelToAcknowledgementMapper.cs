using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class InvalidIlrChangesViewModelToAcknowledgementMapper(
    IOuterApiClient outerApiClient,
    IAuthenticationService authenticationService)
    : IMapper<InvalidIlrChangesViewModel, InvalidIlrChangesAcknowledgementResult>
{
    public async Task<InvalidIlrChangesAcknowledgementResult> Map(InvalidIlrChangesViewModel source)
    {
        var request = new PostInvalidIlrChangesRequest(source.ProviderId, source.ApprenticeshipId, new PostInvalidIlrChangesRequestData
        {
            UserInfo = new ApimUserInfo
            {
                UserId = authenticationService.UserId,
                UserDisplayName = authenticationService.UserName,
                UserEmail = authenticationService.UserEmail
            },
            Acknowledgements = source.RequestSets.ConvertAll(set => new InvalidIlrChangeAcknowledgement
            {
                ApprovalRequestId = set.ApprovalRequestId,
                DeleteAlert = set.DeleteAlert
            })
        });

        await outerApiClient.Post<object>(request);

        return new InvalidIlrChangesAcknowledgementResult();
    }
}
