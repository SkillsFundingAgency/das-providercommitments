using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenPostingDataLockConfirmRestart
{
    private ApprenticeController _sut;
    private Mock<IModelMapper> _modelMapperMock;
    private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
    private DatalockConfirmRestartRequest _request;
    private DatalockConfirmRestartViewModel _viewModel;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _request = fixture.Create<DatalockConfirmRestartRequest>();
        _viewModel = fixture.Create<DatalockConfirmRestartViewModel>();
        _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(x => x.Map<DatalockConfirmRestartViewModel>(_request)).ReturnsAsync(_viewModel);
        _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), _mockCommitmentsApiClient.Object, Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
    }
        
    [TearDown]
    public void TearDown() => _sut.Dispose();


    [Test]
    public void Then_TriageDataLocks_Api_Called()
    {
        //Arrange
        _viewModel.SendRequestToEmployer = true;

        //Act
        _sut.ConfirmRestart(_viewModel);

        //Assert                
        _mockCommitmentsApiClient.Verify(x => x.TriageDataLocks(It.IsAny<long>(), It.IsAny<TriageDataLocksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}