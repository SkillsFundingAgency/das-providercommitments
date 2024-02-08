using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

[TestFixture]
public class WhenPostingDeleteConfirmation
{
    private DraftApprenticeshipController _sut;
    private Mock<ICommitmentsApiClient> _apiClient;
    private Mock<IModelMapper> _modelMapperMock;
    private Mock<IAuthorizationService> _providerFeatureToggle;
    private DeleteConfirmationViewModel _viewModel;
    private DeleteDraftApprenticeshipRequest _mapperResult;

    [SetUp]
    public void Arrange()
    {
        var autoFixture = new Fixture();
        _modelMapperMock = new Mock<IModelMapper>();
        _viewModel = autoFixture.Create<DeleteConfirmationViewModel>();
        _apiClient = new Mock<ICommitmentsApiClient>();
        _apiClient.Setup(x => x.DeleteDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DeleteDraftApprenticeshipRequest>(),
            It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _mapperResult = new DeleteDraftApprenticeshipRequest();
        _modelMapperMock
            .Setup(x => x.Map<DeleteDraftApprenticeshipRequest>(_viewModel))
            .ReturnsAsync(_mapperResult);

        _providerFeatureToggle = new Mock<IAuthorizationService>();
            
        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        _sut = new DraftApprenticeshipController(
            Mock.Of<IMediator>(),
            _apiClient.Object,
            _modelMapperMock.Object,
            Mock.Of<IEncodingService>(),
            _providerFeatureToggle.Object,
            Mock.Of<IOuterApiService>(),
            Mock.Of<IAuthenticationService>());
            
        _sut.TempData = tempData;
    }
    
    [TearDown]
    public void TearDown() => _sut.Dispose();

    [Test]
    public async Task Then_WithValidModel_WithConfirmDeleteTrue_ShouldDeleteDraftApprenticeship()
    {
        //Arrange
        _viewModel.DeleteConfirmed = true;

        //Act
        await _sut.DeleteConfirmation(_viewModel);

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
        await _sut.DeleteConfirmation(_viewModel);

        //Assert           
        var flashMessage = _sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
        Assert.That(flashMessage, Is.Not.Null);
        Assert.That(flashMessage, Is.EqualTo(DraftApprenticeshipController.DraftApprenticeDeleted));
    }

    [Test]
    public async Task Then_WithValidModel_WithConfirmDeleteFalse_ShouldNotDeleteDraftApprenticeship()
    {
        //Arrange
        _viewModel.DeleteConfirmed = false;

        //Act
        await _sut.DeleteConfirmation(_viewModel);

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
        await _sut.DeleteConfirmation(_viewModel);

        //Assert           
        var flashMessage = _sut.TempData[ITempDataDictionaryExtensions.FlashMessageTempDataKey] as string;
        Assert.That(flashMessage, Is.Null);
    }

    [Test]
    public async Task Then_WithValidModel_WithConfirmDeleteFalse_ShouldRedirectToEditApprenticeship()
    {
        //Arrange
        _viewModel.DeleteConfirmed = false;

        //Act
        var result = await _sut.DeleteConfirmation(_viewModel);

        //Assert           
        var redirect = result.VerifyReturnsRedirectToActionResult();
        Assert.That(redirect.ActionName, Is.EqualTo("Details"));
        Assert.That(redirect.ControllerName, Is.EqualTo("Cohort"));
    }

    [Test]
    public async Task Then_WithValidModel_WithConfirmDeleteTrue_ShouldRedirectToCohortDetailsPage()
    {
        //Arrange
        _viewModel.DeleteConfirmed = true;

        //Act
        var result = await _sut.DeleteConfirmation(_viewModel);

        //Assert           
        result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
    }
}