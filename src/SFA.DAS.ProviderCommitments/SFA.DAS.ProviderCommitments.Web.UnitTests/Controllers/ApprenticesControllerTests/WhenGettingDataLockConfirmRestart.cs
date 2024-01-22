using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingDataLockConfirmRestart
    {
        private ApprenticeController _sut;
        private DatalockConfirmRestartRequest _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DatalockConfirmRestartRequest>();
            _sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        }
        
        [TearDown]
        public void TearDown() => _sut.Dispose();

        [Test]
        public void ThenReturnsView()
        {
            //Act
            var result = _sut.ConfirmRestart(_request) as ViewResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model.GetType(), Is.EqualTo(typeof(DatalockConfirmRestartViewModel)));
        }
    }
}
