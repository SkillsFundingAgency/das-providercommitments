using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingUpdateDataLock
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
            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            //Act
            await _sut.UpdateDataLock(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<UpdateDateLockViewModel>(_request));
        }      

        [Test]
        public async Task ThenReturnsView()
        {
            //Act
            var result =await _sut.UpdateDataLock(_request) as ViewResult;

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(typeof(UpdateDateLockViewModel), result.Model.GetType());
        }
    }
}
