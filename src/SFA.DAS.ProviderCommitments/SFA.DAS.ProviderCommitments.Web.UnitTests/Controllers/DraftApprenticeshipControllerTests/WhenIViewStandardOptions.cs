namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class WhenIViewStandardOptions
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task Then_If_There_Are_No_Options_Then_Redirect_To_Draft()
        {
            await _fixture
                .ReturnNoMappedOptions()
                .ViewStandardOptions();
            
            _fixture.VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task Then_If_There_Are_Options_Then_The_View_Is_Shown()
        {
            await _fixture
                .ViewStandardOptions();

            _fixture.VerifySelectOptionsViewReturned();
        }
    }
}