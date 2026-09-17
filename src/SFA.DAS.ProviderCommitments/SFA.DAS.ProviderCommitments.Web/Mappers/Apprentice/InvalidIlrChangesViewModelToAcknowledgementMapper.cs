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
        await AcknowledgeUnacknowledgedApprovalChanges.Post(
            outerApiClient,
            authenticationService,
            source,
            GetInvalidIlrChangesRequest.InvalidIlrChangesPath);

        return new InvalidIlrChangesAcknowledgementResult();
    }
}

public class DeclinedChangesViewModelToAcknowledgementMapper(
    IOuterApiClient outerApiClient,
    IAuthenticationService authenticationService)
    : IMapper<DeclinedChangesViewModel, DeclinedChangesAcknowledgementResult>
{
    public async Task<DeclinedChangesAcknowledgementResult> Map(DeclinedChangesViewModel source)
    {
        await AcknowledgeUnacknowledgedApprovalChanges.Post(
            outerApiClient,
            authenticationService,
            source,
            GetInvalidIlrChangesRequest.DeclinedChangesPath);

        return new DeclinedChangesAcknowledgementResult();
    }
}

public static class AcknowledgeUnacknowledgedApprovalChanges
{
    public static Task Post(
        IOuterApiClient outerApiClient,
        IAuthenticationService authenticationService,
        InvalidIlrChangesViewModel source,
        string path)
    {
        var request = new PostInvalidIlrChangesRequest(
            source.ProviderId,
            source.ApprenticeshipId,
            new PostInvalidIlrChangesRequestData
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
            },
            path);

        return outerApiClient.Post<object>(request);
    }
}
