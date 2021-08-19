using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authentication;

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
            result.VerifyReturnsRedirectToActionResult().WithActionName("Cohorts");
        }
    }

    public class PostDeleteCohortFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<IModelMapper> _mockModelMapper;
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

            _mockModelMapper = new Mock<IModelMapper>();
            _authenticationService = new Mock<IAuthenticationService>();

            _authenticationService.Setup(x => x.UserInfo).Returns(_userInfo);

            _commitmentApiClient
                .Setup(x => x.DeleteCohort(_viewModel.CohortId, _userInfo , It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            RedirectUrl = $"{_viewModel.ProviderId}/apprentices/{_viewModel.CohortReference}/Details";
            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(RedirectUrl)).Returns(RedirectUrl);

            Sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, _linkGenerator.Object, _commitmentApiClient.Object, Mock.Of<IEncodingService>());
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

        public async Task<IActionResult> Act() => await Sut.Delete(_authenticationService.Object, _viewModel);
    }
}
