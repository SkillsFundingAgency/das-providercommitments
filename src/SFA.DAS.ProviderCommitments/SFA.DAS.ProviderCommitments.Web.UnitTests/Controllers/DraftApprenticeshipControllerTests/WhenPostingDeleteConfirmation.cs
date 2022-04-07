using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using AutoFixture;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenPostingDeleteConfirmation
    {
        public DraftApprenticeshipController Sut { get; set; }
        private string RedirectUrl;
        private Mock<ICommitmentsApiClient> _apiClient;
        private Mock<IModelMapper> _modelMapperMock;
        private Mock<IAuthorizationService> _providerFeatureToggle;
        private DeleteConfirmationViewModel _viewModel;
        private DeleteDraftApprenticeshipRequest _mapperResult;

        [SetUp]
        public void Arrange()
        {
            var _autoFixture = new Fixture();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = _autoFixture.Create<DeleteConfirmationViewModel>();
            _apiClient = new Mock<ICommitmentsApiClient>();
            _apiClient.Setup(x => x.DeleteDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DeleteDraftApprenticeshipRequest>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            _mapperResult = new DeleteDraftApprenticeshipRequest();
            _modelMapperMock
              .Setup(x => x.Map<DeleteDraftApprenticeshipRequest>(_viewModel))
              .ReturnsAsync(_mapperResult);

            _providerFeatureToggle = new Mock<IAuthorizationService>();

            RedirectUrl = $"{_viewModel.ProviderId}/apprentices/{_viewModel.CohortReference}/Details";
         
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            Sut = new DraftApprenticeshipController(Mock.Of<IMediator>(), _apiClient.Object, _modelMapperMock.Object, Mock.Of<IEncodingService>(), _providerFeatureToggle.Object);
            Sut.TempData = tempData;
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteTrue_ShouldDeleteDraftApprenticeship()
        {
            //Arrange
            _viewModel.DeleteConfirmed = true;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            _apiClient.Verify(x => x.DeleteDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(),
              It.IsAny<DeleteDraftApprenticeshipRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteTrue_ShouldStoreSuccessMessageInTempData()
        {
            //Arrange
            _viewModel.DeleteConfirmed = true;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            var flashMessage = Sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
            Assert.NotNull(flashMessage);
            Assert.AreEqual(flashMessage, DraftApprenticeshipController.DraftApprenticeDeleted);
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteFalse_ShouldNotDeleteDraftApprenticeship()
        {
            //Arrange
            _viewModel.DeleteConfirmed = false;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            _apiClient.Verify(x => x.DeleteDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(),
              It.IsAny<DeleteDraftApprenticeshipRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteFalse_ShouldNotStoreSuccessMessageInTempData()
        {
            //Arrange
            _viewModel.DeleteConfirmed = false;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            var flashMessage = Sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
            Assert.IsNull(flashMessage);
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteFalse_ShouldRedirectToEditApprenticeship()
        {
            //Arrange
            _viewModel.DeleteConfirmed = false;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            var redirect = result.VerifyReturnsRedirectToActionResult();
            Assert.AreEqual("ViewEditDraftApprenticeship", redirect.ActionName);
            Assert.AreEqual("DraftApprenticeship", redirect.ControllerName);
        }

        [Test]
        public async Task Then_WithValidModel_WithConfirmDeleteTrue_ShouldRedirectToCohortDetailsPage()
        {
            //Arrange
            _viewModel.DeleteConfirmed = true;

            //Act
            var result = await Sut.DeleteConfirmation(_viewModel);

            //Assert           
            result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        }
    }
}
