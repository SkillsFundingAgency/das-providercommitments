using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingDeleteCohort
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new WhenGettingDeleteCohortFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingDeleteCohortFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<DeleteCohortViewModel>();
        }
    }

    public class WhenGettingDeleteCohortFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly DeleteCohortRequest _request;

        public WhenGettingDeleteCohortFixture()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DeleteCohortRequest>();
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<DeleteCohortViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<DeleteCohortViewModel>(_request))
                .ReturnsAsync(viewModel);
            
            _sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<DeleteCohortViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.Delete(_request);
    }
}
