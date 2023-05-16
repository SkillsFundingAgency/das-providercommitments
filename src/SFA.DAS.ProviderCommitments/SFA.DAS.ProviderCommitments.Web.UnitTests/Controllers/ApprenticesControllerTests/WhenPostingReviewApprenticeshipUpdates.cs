using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingReviewApprenticeshipUpdates
    {
        private WhenPostingReviewApprenticeshipUpdatesFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingReviewApprenticeshipUpdatesFixture();
        }

        [Test]
        public async Task WithAddThisStandard_RedirectTo_review_your_details()
        {
            _fixture = _fixture.WithIsValidCourseCode(false);

            _fixture = _fixture.WithAddThisStandard(true);

            var result = await _fixture.Act();

            var redirect = result.VerifyReturnsRedirect();
            redirect.Url.Should().Contain($"/{_fixture._viewModel.ProviderId}/review-your-details");
        }

        [Test]
        public async Task Without_ValidCourse_And_RejectChanges_RejectChanges_Is_Called()
        {
            _fixture = _fixture.WithIsValidCourseCode(false);
            _fixture = _fixture.WithAddThisStandard(false);

            await _fixture.Act();

            _fixture.VerifyRejectChangesCalled();
        }

        [Test]
        public async Task WithAcceptChanges_AcceptChanges_Is_Called()
        {
            _fixture = _fixture.WithAcceptChanges();

            await _fixture.Act();

            _fixture.VerifyAcceptChangesCalled();
        }

        [Test]
        public async Task WithRejectChanges_RejectChanges_Is_Called()
        {
            _fixture = _fixture.WithRejectChanges();

            await _fixture.Act();

            _fixture.VerifyRejectChangesCalled();
        }

        [Test]
        public async Task WithAcceptChanges_ChangesApproved_Message_Is_Stored_In_TempData()
        {
            _fixture = _fixture.WithAcceptChanges();

            await _fixture.Act();

            _fixture.VerifyChangesApprovedFlashMessageStoredInTempData();
        }

        [Test]
        public async Task WithRejectChanges_Changes_Rejected_Message_Is_Stored_In_TempData()
        {
            _fixture = _fixture.WithRejectChanges();

            await _fixture.Act();

            _fixture.VerifyChangesRejectedFlashMessageStoredInTempData();
        }

        [Test]
        public async Task WithAcceptChanges_RedirectToApprenticeDetails()
        {
            _fixture = _fixture.WithAcceptChanges();

            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        [Test]
        public async Task WithRejectChanges_RedirectToApprenticeDetails()
        {
            _fixture = _fixture.WithRejectChanges();

            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        internal class WhenPostingReviewApprenticeshipUpdatesFixture
        {
            private readonly ApprenticeController _sut;
            public readonly ReviewApprenticeshipUpdatesViewModel _viewModel;
            private readonly UndoApprenticeshipUpdatesRequest _mapperResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IModelMapper> _modelMapper;
            protected Mock<ILinkGenerator> _mockLinkGenerator;
            public UserInfo UserInfo;
            public Mock<IAuthenticationService> AuthenticationService { get; }

            public WhenPostingReviewApprenticeshipUpdatesFixture()
            {
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.UndoApprenticeshipUpdates(It.IsAny<long>(), It.IsAny<UndoApprenticeshipUpdatesRequest>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                _modelMapper = new Mock<IModelMapper>();

                _mockLinkGenerator = new Mock<ILinkGenerator>();

                _mockLinkGenerator.Setup(x => x.CourseManagementLink(It.IsAny<string>()))
                         .Returns((string url) => "http://coursemanagement/" + url);

                _viewModel = new ReviewApprenticeshipUpdatesViewModel
                {
                    ApprenticeshipId = 123,
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342,
                    IsValidCourseCode = true
                };

                var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                var autoFixture = new Fixture();
                UserInfo = autoFixture.Create<UserInfo>();
                AuthenticationService = new Mock<IAuthenticationService>();
                AuthenticationService.Setup(x => x.UserInfo).Returns(UserInfo);

                _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), _apiClient.Object);

                _sut.TempData = tempData;
            }

            public Task<IActionResult> Act() => _sut.ReviewApprenticeshipUpdates(AuthenticationService.Object, _viewModel, _mockLinkGenerator.Object);

            public WhenPostingReviewApprenticeshipUpdatesFixture WithIsValidCourseCode(bool isValidCourseCode)
            {
                _viewModel.IsValidCourseCode = isValidCourseCode;
                return this;
            }

            public WhenPostingReviewApprenticeshipUpdatesFixture WithAddThisStandard(bool? addThisStandard)
            {
                _viewModel.AddThisStandard = addThisStandard;
                return this;
            }
            public WhenPostingReviewApprenticeshipUpdatesFixture WithAcceptChanges()
            {
                _viewModel.ApproveChanges = true;
                return this;
            }

            public WhenPostingReviewApprenticeshipUpdatesFixture WithRejectChanges()
            {
                _viewModel.ApproveChanges = false;
                return this;
            }

            public void VerifyAcceptChangesCalled()
            {
                _apiClient.Verify(x => x.AcceptApprenticeshipUpdates(It.Is<long>(id => id == _viewModel.ApprenticeshipId),
                    It.Is<AcceptApprenticeshipUpdatesRequest>(o => o.UserInfo != null),
                    It.IsAny<CancellationToken>()));
            }

            public void VerifyRejectChangesCalled()
            {
                _apiClient.Verify(x => x.RejectApprenticeshipUpdates(It.Is<long>(id => id == _viewModel.ApprenticeshipId),
                    It.Is<RejectApprenticeshipUpdatesRequest>(o => o.UserInfo != null),
                    It.IsAny<CancellationToken>()));
            }

            public void VerifyChangesApprovedFlashMessageStoredInTempData()
            {
                var flashMessage = _sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
                Assert.NotNull(flashMessage);
                Assert.AreEqual(flashMessage, ApprenticeController.ChangesApprovedFlashMessage);
            }

            public void VerifyChangesRejectedFlashMessageStoredInTempData()
            {
                var flashMessage = _sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
                Assert.NotNull(flashMessage);
                Assert.AreEqual(flashMessage, ApprenticeController.ChangesRejectedFlashMessage);
            }
        }
    }
}
