using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingViewApprenticeshipUpdates
    {
        private WhenPostingViewApprenticeshipUpdatesFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingViewApprenticeshipUpdatesFixture();
        }

        [Test]
        public async Task WithUndoChanges_UndoChanges_Is_Called()
        {
            _fixture = _fixture.WithUndoChanges();

            await _fixture.Act();

            _fixture.VerifyUndoChangesCalled();
        }

        [Test]
        public async Task WithUndoChanges_Changes_Undone_Message_Is_Stored_In_TempData()
        {
            _fixture = _fixture.WithUndoChanges();

            await _fixture.Act();

            _fixture.VerifyChangesUndoneFlashMessageStoredInTempData();
        }

        [Test]
        public async Task WithUndoChanges_RedirectToSent()
        {
            _fixture = _fixture.WithUndoChanges();

            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        [Test]
        public async Task WithLeaveChanges_RedirectToSent()
        {
            _fixture = _fixture.WithLeaveChanges();

            var result = await _fixture.Act();

            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        internal class WhenPostingViewApprenticeshipUpdatesFixture
        {
            private readonly ApprenticeController _sut;
            private readonly ViewApprenticeshipUpdatesViewModel _viewModel;
            private readonly UndoApprenticeshipUpdatesRequest _mapperResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private Mock<IAuthenticationService> AuthenticationService { get; }

            public WhenPostingViewApprenticeshipUpdatesFixture()
            {
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.UndoApprenticeshipUpdates(It.IsAny<long>(), It.IsAny<UndoApprenticeshipUpdatesRequest>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                var modelMapper = new Mock<IModelMapper>();

                _viewModel = new ViewApprenticeshipUpdatesViewModel
                {
                    ApprenticeshipId = 123,
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342
                };

                var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
                var autoFixture = new Fixture();
                var userInfo = autoFixture.Create<UserInfo>();
                AuthenticationService = new Mock<IAuthenticationService>();
                AuthenticationService.Setup(x => x.UserInfo).Returns(userInfo);

                _sut = new ApprenticeController(modelMapper.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), _apiClient.Object, Mock.Of<IOuterApiService>());

                _sut.TempData = tempData;
            }

            public Task<IActionResult> Act() => _sut.ViewApprenticeshipUpdates(AuthenticationService.Object, _viewModel);

            public WhenPostingViewApprenticeshipUpdatesFixture WithUndoChanges()
            {
                _viewModel.UndoChanges = true;
                return this;
            }

            public WhenPostingViewApprenticeshipUpdatesFixture WithLeaveChanges()
            {
                _viewModel.UndoChanges = false;
                return this;
            }

            public void VerifyUndoChangesCalled()
            {
                _apiClient.Verify(x => x.UndoApprenticeshipUpdates(It.Is<long>(id => id == _viewModel.ApprenticeshipId),
                    It.Is<UndoApprenticeshipUpdatesRequest>(o => o.UserInfo != null),
                    It.IsAny<CancellationToken>()));
            }

            public void VerifyChangesUndoneFlashMessageStoredInTempData()
            {
                var flashMessage = _sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
                Assert.That(flashMessage, Is.Not.Null);
                Assert.That(ApprenticeController.ChangesUndoneFlashMessage, Is.EqualTo(flashMessage));
            }
        }
    }
}
