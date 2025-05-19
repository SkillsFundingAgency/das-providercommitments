using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenLearnerSelected
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new LearnerSelectedFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new LearnerSelectedFixture();

            var result = await fixture.Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("AddDraftApprenticeship");
        }
    }

    public class LearnerSelectedFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly LearnerSelectedRequest _request;
        private readonly CreateCohortWithDraftApprenticeshipRequest _model;
        private readonly long _providerId;
        private readonly long _learnerDataId;

        public LearnerSelectedFixture()
        {
            var fixture = new Fixture();
            _providerId = fixture.Create<long>();
            _learnerDataId = fixture.Create<long>();
            _model = fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
            _request = new LearnerSelectedRequest { ProviderId = _providerId, LearnerDataId = _learnerDataId };
            _modelMapperMock = new Mock<IModelMapper>();

            _modelMapperMock
                .Setup(x => x.Map<CreateCohortWithDraftApprenticeshipRequest>(_request))
                .ReturnsAsync(_model);
            
            _sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<CreateCohortWithDraftApprenticeshipRequest>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.LearnerSelected(_request);
    }
}