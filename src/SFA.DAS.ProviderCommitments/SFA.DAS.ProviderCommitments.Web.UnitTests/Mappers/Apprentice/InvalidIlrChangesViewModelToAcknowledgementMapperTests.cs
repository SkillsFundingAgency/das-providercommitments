using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class InvalidIlrChangesViewModelToAcknowledgementMapperTests
{
    [Test, MoqAutoData]
    public async Task Map_ThenPostsKeepAndDeleteChoices(
        InvalidIlrChangesViewModel viewModel,
        [Frozen] Mock<IOuterApiClient> outerApiClient,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        InvalidIlrChangesViewModelToAcknowledgementMapper mapper)
    {
        authenticationService.Setup(x => x.UserId).Returns("user-1");
        authenticationService.Setup(x => x.UserName).Returns("Jane Doe");
        authenticationService.Setup(x => x.UserEmail).Returns("jane@example.com");

        viewModel.RequestSets =
        [
            new InvalidIlrChangeSetViewModel
            {
                ApprovalRequestId = Guid.NewGuid(),
                DeleteAlert = true
            },
            new InvalidIlrChangeSetViewModel
            {
                ApprovalRequestId = Guid.NewGuid(),
                DeleteAlert = false
            }
        ];

        outerApiClient.Setup(x => x.Post<object>(It.IsAny<PostInvalidIlrChangesRequest>()))
            .ReturnsAsync((object)null);

        await mapper.Map(viewModel);

        outerApiClient.Verify(x => x.Post<object>(
            It.Is<PostInvalidIlrChangesRequest>(request =>
                request.PostUrl == $"{viewModel.ProviderId}/apprentices/{viewModel.ApprenticeshipId}/invalid-ilr-changes" &&
                ((PostInvalidIlrChangesRequestData)request.Data).UserInfo.UserId == "user-1" &&
                ((PostInvalidIlrChangesRequestData)request.Data).Acknowledgements.Count == 2 &&
                ((PostInvalidIlrChangesRequestData)request.Data).Acknowledgements[0].DeleteAlert == true &&
                ((PostInvalidIlrChangesRequestData)request.Data).Acknowledgements[1].DeleteAlert == false)),
            Times.Once);
    }
}
