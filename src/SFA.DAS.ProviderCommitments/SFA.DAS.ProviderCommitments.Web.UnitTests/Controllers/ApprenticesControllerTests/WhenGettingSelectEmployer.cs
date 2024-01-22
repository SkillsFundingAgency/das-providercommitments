using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingSelectEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new SelectEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new SelectEmployerFixture();

            var result = await fixture.Act() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model.GetType(), Is.EqualTo(typeof(SelectEmployerViewModel)));
        }
    }

    public class SelectEmployerFixture
    {
        private readonly ApprenticeController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly SelectEmployerRequest _request;

        public SelectEmployerFixture()
        {
            _request = new SelectEmployerRequest { ProviderId = 1, ApprenticeshipId = 1 };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityViewModel>(),
            };

            _modelMapperMock
                .Setup(x => x.Map<SelectEmployerViewModel>(_request))
                .ReturnsAsync(viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<SelectEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.SelectEmployer(_request);
    }
}
