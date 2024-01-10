using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingConfirmDataLockChanges
    {
        private ApprenticeController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private ConfirmDataLockChangesRequest _request;
        private ConfirmDataLockChangesViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<ConfirmDataLockChangesRequest>();
            _viewModel = fixture.Create<ConfirmDataLockChangesViewModel>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<ConfirmDataLockChangesViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), _mockCommitmentsApiClient.Object, Mock.Of<IOuterApiService>());
        }
        
        [TearDown]
        public void TearDown() => _sut.Dispose();

        [Test]
        public void Then_TriageDataLocks_Api_Called()
        {
            //Arrange
            _viewModel.SubmitStatusViewModel = SubmitStatusViewModel.Confirm;

            //Act
            var result = _sut.ConfirmDataLockChanges(_viewModel);

            //Assert                
            _mockCommitmentsApiClient.Verify(x => x.TriageDataLocks(It.IsAny<long>(), It.IsAny<TriageDataLocksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
