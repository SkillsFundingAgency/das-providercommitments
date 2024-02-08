using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

[TestFixture]
public class WhenPostingUpdateDataLock
{
    private ApprenticeController _sut;
    private Mock<IModelMapper> _modelMapperMock;
    private UpdateDateLockRequest _request;
    private UpdateDateLockViewModel _viewModel;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _request = fixture.Create<UpdateDateLockRequest>();
        _viewModel = fixture.Create<UpdateDateLockViewModel>();
        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(x => x.Map<UpdateDateLockViewModel>(_request)).ReturnsAsync(_viewModel);
        _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
    }
        
    [TearDown]
    public void TearDown() => _sut.Dispose();

    [Test]
    public void Then_Redirect_To_Details_Page()
    {
        //Act
        var result = _sut.UpdateDataLock(_viewModel);

        //Assert
        result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
    }

    [Test]
    public void Then_Redirect_To_ConfirmRestart_Page()
    {
        //Arrange
        _viewModel.SubmitStatusViewModel = SubmitStatusViewModel.Confirm;

        //Act
        var result = _sut.UpdateDataLock(_viewModel);

        //Assert
        result.VerifyReturnsRedirectToActionResult().WithActionName("ConfirmDataLockChanges");
    }
}