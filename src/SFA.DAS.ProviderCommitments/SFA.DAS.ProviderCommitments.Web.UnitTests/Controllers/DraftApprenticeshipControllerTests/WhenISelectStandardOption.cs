namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class WhenISelectStandardOption
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task Then_The_Option_Is_Saved_And_Redirected_To_Cohort_Details()
        {
            _fixture.SetupUpdateRequestCourseOption();
                
            await _fixture.PostToSelectOption();
            
            _fixture
                .VerifyApiUpdateWithStandardOptionSet()
                .VerifyRedirectedToRplQuestion();
        }
    }
}