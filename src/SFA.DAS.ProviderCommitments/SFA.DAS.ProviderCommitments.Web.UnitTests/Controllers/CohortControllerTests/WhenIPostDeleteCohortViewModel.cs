using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostDeleteCohortViewModel
    {
        [Test]
        public async Task PostDeleteCohortViewModel_WithValidModel_WithConfirmFalse_ShouldRedirectToSelectEmployer()
        {
            var fixture = new PostDeleteCohortFixture()
                .WithConfirmFalse();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("Details"); ;
         
        }

        [Test]
        public async Task PostDeleteCohortViewModel_WithValidModel_WithConfirmTrue_ShouldDeleteCohort()
        {
            var fixture = new PostDeleteCohortFixture()
                .WithConfirmTrue();

            await fixture.Act();
            fixture.VerifyCohortDeleted();
        }


            [Test]
        public async Task PostDeleteCohortViewModel_WithValidModel_WithConfirmTrue_ShouldRedirectToCohortsPage()
        {
            var fixture = new PostDeleteCohortFixture()
                .WithConfirmTrue();

            var result = await fixture.Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("Review");
        }
    }

    public class PostDeleteCohortFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClient;
        private readonly DeleteCohortViewModel _viewModel;
        private readonly UserInfo _userInfo;
        private readonly Mock<IAuthenticationService> _authenticationService;

        public PostDeleteCohortFixture()
        {
            var fixture = new Fixture();
            _viewModel = fixture.Create<DeleteCohortViewModel>();
            _userInfo = fixture.Create<UserInfo>();
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();

            var mockModelMapper = new Mock<IModelMapper>();
            _authenticationService = new Mock<IAuthenticationService>();

            _authenticationService.Setup(x => x.UserInfo).Returns(_userInfo);

            _commitmentApiClient
                .Setup(x => x.DeleteCohort(_viewModel.CohortId, _userInfo , It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            var redirectUrl = $"{_viewModel.ProviderId}/apprentices/{_viewModel.CohortReference}/Details";
            var linkGenerator = new Mock<ILinkGenerator>();
            linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(redirectUrl)).Returns(redirectUrl);

            _sut = new CohortController(Mock.Of<IMediator>(), mockModelMapper.Object, linkGenerator.Object, _commitmentApiClient.Object, 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }

        public PostDeleteCohortFixture WithConfirmFalse()
        {
            _viewModel.Confirm = false;
            return this;
        }

        public PostDeleteCohortFixture WithConfirmTrue()
        {
            _viewModel.Confirm = true;
            return this;
        }


        public PostDeleteCohortFixture VerifyCohortDeleted()
        {
            _commitmentApiClient.Verify(x => x.DeleteCohort(_viewModel.CohortId, _userInfo, It.IsAny<CancellationToken>()), Times.Once);
            return this;
        }

        public async Task<IActionResult> Act() => await _sut.Delete(_authenticationService.Object, _viewModel);
    }
}
