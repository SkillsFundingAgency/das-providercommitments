using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

[TestFixture]
public class WhenPostingRecognisePriorLearningSummaryRequest
{
    private DraftApprenticeshipController _sut;
    private Mock<IModelMapper> _modelMapperMock;
    private Mock<IAuthorizationService> _providerFeatureToggle;
    private PriorLearningSummaryViewModel _viewModel;

    [SetUp]
    public void Arrange()
    {
        var autoFixture = new Fixture();
        _modelMapperMock = new Mock<IModelMapper>();
        _viewModel = autoFixture.Create<PriorLearningSummaryViewModel>();

        _providerFeatureToggle = new Mock<IAuthorizationService>();
        _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

        _sut = new DraftApprenticeshipController(
            Mock.Of<IMediator>(),
            Mock.Of<ICommitmentsApiClient>(),
            _modelMapperMock.Object,
            Mock.Of<IEncodingService>(),
            _providerFeatureToggle.Object,
            Mock.Of<IOuterApiService>(),
            Mock.Of<IAuthenticationService>(),
            Mock.Of<ICacheStorageService>()
        );
    }

    [TearDown]
    public void TearDown() => _sut.Dispose();
    
    [Test]
    public void When_posting_from_Recognise_Prior_Learning_Summary()
    {
        var action = _sut.RecognisePriorLearningSummary(_viewModel);
        action.VerifyReturnsRedirectToActionResult().WithActionName("Details");
    }
}