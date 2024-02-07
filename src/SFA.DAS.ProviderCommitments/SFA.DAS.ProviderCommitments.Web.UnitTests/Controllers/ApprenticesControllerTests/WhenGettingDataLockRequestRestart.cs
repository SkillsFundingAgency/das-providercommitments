using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingDataLockRequestRestart
    {
        private ApprenticeController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private DataLockRequestRestartRequest _request;
        private DataLockRequestRestartViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DataLockRequestRestartRequest>();
            _viewModel = fixture.Create<DataLockRequestRestartViewModel>();
            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<DataLockRequestRestartViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>(), Mock.Of<ILogger<ApprenticeController>>());
        }
        
        [TearDown]
        public void TearDown() => _sut.Dispose();

        [Test]
        public async Task ThenCallsModelMapper()
        {
            //Act
            await _sut.DataLockRequestRestart(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<DataLockRequestRestartViewModel>(_request));
        }

        [Test]
        public async Task ThenReturnsView()
        {
            //Act
            var result = await _sut.DataLockRequestRestart(_request) as ViewResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model.GetType(), Is.EqualTo(typeof(DataLockRequestRestartViewModel)));
        }
    }
}
