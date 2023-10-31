using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingSentTests
    {
        private string _newEmployerName;
        private ApprenticeController _sut;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _newEmployerName = fixture.Create<string>();
            _sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData[nameof(ConfirmViewModel.NewEmployerName)] = _newEmployerName;
            _sut.TempData = tempData;
        }

        [Test]
        public void ThenNewEmployerNameIsPopulatedFromTempData()
        {
            var result = _sut.Sent() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(_newEmployerName, result.Model as string);
        }
    }
}