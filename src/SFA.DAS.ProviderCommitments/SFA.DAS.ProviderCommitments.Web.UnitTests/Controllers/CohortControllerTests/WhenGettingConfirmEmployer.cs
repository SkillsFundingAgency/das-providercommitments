using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingConfirmEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetConfirmEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetConfirmEmployerFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ConfirmEmployerViewModel>();
        }
    }

    public class GetConfirmEmployerFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ConfirmEmployerRequest _request;
        private readonly long _providerId;

        public GetConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            _request = new ConfirmEmployerRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<ConfirmEmployerViewModel>();
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<ConfirmEmployerViewModel>(_request))
                .ReturnsAsync(viewModel);
            
            _sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.ConfirmEmployer(_request);
    }
}
