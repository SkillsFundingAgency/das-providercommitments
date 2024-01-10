using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenGettingRecognisePriorLearningSummaryRequest
    {
        private DraftApprenticeshipController _sut;
        private Mock<IModelMapper> _modelMapperMock;
        private Mock<IAuthorizationService> _providerFeatureToggle;
        private PriorLearningSummaryViewModel _viewModel;
        private PriorLearningSummaryRequest _request;

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();
            _request = autoFixture.Create<PriorLearningSummaryRequest>();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = autoFixture.Create<PriorLearningSummaryViewModel>();

            _modelMapperMock
              .Setup(x => x.Map<PriorLearningSummaryViewModel>(_request))
              .ReturnsAsync(_viewModel);

            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            _sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                Mock.Of<ICommitmentsApiClient>(),
                _modelMapperMock.Object,
                Mock.Of<IEncodingService>(),
                _providerFeatureToggle.Object,
                Mock.Of<IOuterApiService>(),
                Mock.Of<IAuthenticationService>()
                );
        }

        [Test]
        public async Task Then_Call_ModelMapper()
        {
            //Act
            await _sut.RecognisePriorLearningSummary(_request);

            //Assert
            _modelMapperMock.Verify(x => x.Map<PriorLearningSummaryViewModel>(_request));
        }

        [Test]
        public async Task Then_Returns_View()
        {
            //Act
            var result = await _sut.RecognisePriorLearningSummary(_request);

            //Assert
            result.VerifyReturnsViewModel().WithModel<PriorLearningSummaryViewModel>();
        }
    }
}