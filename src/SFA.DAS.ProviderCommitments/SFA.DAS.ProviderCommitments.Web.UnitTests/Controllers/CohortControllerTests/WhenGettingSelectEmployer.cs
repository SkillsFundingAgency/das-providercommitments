using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
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
        public CohortController Sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly SelectEmployerRequest _request;
        private readonly long _providerId;

        public SelectEmployerFixture()
        {
            _request = new SelectEmployerRequest { ProviderId = _providerId };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityViewModel>(),
                BackLink = "Test.com"
            };
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<SelectEmployerViewModel>(_request))
                .ReturnsAsync(viewModel);

            Sut = new CohortController(Mock.Of<IMediator>(), _modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public SelectEmployerFixture WithModelStateErrors()
        {
            Sut.ControllerContext.ModelState.AddModelError("TestError","Test Error");
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<SelectEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.SelectEmployer(_request);
    }
}
