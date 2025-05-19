using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingSelectLearner
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetSelectLearnerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetSelectLearnerFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<SelectLearnerRecordViewModel>();
        }
    }

    public class GetSelectLearnerFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly SelectLearnerRecordRequest _request;
        private readonly long _providerId;

        public GetSelectLearnerFixture()
        {
            var fixture = new Fixture();
            _request = new SelectLearnerRecordRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<SelectLearnerRecordViewModel>();
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<SelectLearnerRecordViewModel>(_request))
                .ReturnsAsync(viewModel);
            
            _sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<SelectLearnerRecordViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.SelectLearnerRecord(_request);
    }
}
