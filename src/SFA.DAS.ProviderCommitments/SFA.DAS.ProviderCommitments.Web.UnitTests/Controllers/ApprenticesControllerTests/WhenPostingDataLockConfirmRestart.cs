using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
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
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), _mockCommitmentsApiClient.Object);
        }


        [Test]
        public void Then_TriageDataLocks_Api_Called()
        {
            //Arrange
            _viewModel.SendRequestToEmployer = true;

            //Act
            var result = _sut.ConfirmRestart(_viewModel);

            //Assert                
            _mockCommitmentsApiClient.Verify(x => x.TriageDataLocks(It.IsAny<long>(), It.IsAny<TriageDataLocksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
