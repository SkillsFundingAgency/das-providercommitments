using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System.Threading;
using System.Threading.Tasks;

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
            await _fixture.Act();
            _fixture.VerifyUndoChangesCalled();
        }

        [Test]
        public async Task WithUndoChanges_Changes_Undone_Message_Is_Stored_In_TempData()
        {
            await _fixture.Act();

            _fixture.VerifyChangesUndoneFlashMessageStoredInTempData();
        }

        [Test]
        public async Task WithUndoChanges_RedirectToSent()
        {
            var result = await _fixture.Act();
            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        [Test]
        public async Task WithLeaveChanges_RedirectToSent()
        {
            var result = await _fixture.Act();
            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeDetail);
        }

        internal class WhenPostingViewApprenticeshipUpdatesFixture
        {
            private readonly ApprenticeController _sut;
            private readonly ViewApprenticeshipUpdatesViewModel _viewModel;
            private readonly UndoApprenticeshipUpdatesRequest _mapperResult;
            private readonly Mock<ICommitmentsApiClient> _apiClient;
            private readonly Mock<IModelMapper> _modelMapper;

            public WhenPostingViewApprenticeshipUpdatesFixture()
            {
                _apiClient = new Mock<ICommitmentsApiClient>();
                _apiClient.Setup(x => x.UndoApprenticeshipUpdates(It.IsAny<long>(), It.IsAny<UndoApprenticeshipUpdatesRequest>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                _mapperResult = new UndoApprenticeshipUpdatesRequest();

                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<UndoApprenticeshipUpdatesRequest>(It.IsAny<ViewApprenticeshipUpdatesViewModel>()))
                    .ReturnsAsync(_mapperResult);

                _viewModel = new ViewApprenticeshipUpdatesViewModel
                {
                    ApprenticeshipId = 123,
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342
                };

                var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                _sut = new ApprenticeController(_modelMapper.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), _apiClient.Object);

                _sut.TempData = tempData;
            }

            public Task<IActionResult> Act() => _sut.ViewApprenticeshipUpdates(_viewModel);

            public void VerifyUndoChangesCalled()
            {
                _apiClient.Verify(x => x.UndoApprenticeshipUpdates(It.Is<long>(id => id == _viewModel.ApprenticeshipId),
                    It.Is<UndoApprenticeshipUpdatesRequest>(r => r == _mapperResult),
                    It.IsAny<CancellationToken>()));
            }

            public void VerifyChangesUndoneFlashMessageStoredInTempData()
            {
                var flashMessage = _sut.TempData[nameof(ApprenticeController.ChangesUndoneFlashMessage)] as string;
                Assert.NotNull(flashMessage);
                Assert.AreEqual(flashMessage, ApprenticeController.ChangesUndoneFlashMessage);
            }
        }
    }
}
