using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using AutoFixture;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using System.Threading;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingDeleteConfirmation
    {
        public ApprenticeController Sut { get; set; }
        private string RedirectUrl;
        private Mock<ICommitmentsApiClient> _apiClient;
        private Mock<IModelMapper> _modelMapperMock;
        private DeleteConfirmationViewModel _viewModel;        
        private Mock<ILinkGenerator> _linkGenerator;
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

            RedirectUrl = $"{_viewModel.ProviderId}/apprentices/{_viewModel.CommitmentHashedId}/Details"; 
            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(RedirectUrl)).Returns(RedirectUrl);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), _linkGenerator.Object, _apiClient.Object);
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
            result.VerifyReturnsRedirect().WithUrl(RedirectUrl);
        }
    }
}
