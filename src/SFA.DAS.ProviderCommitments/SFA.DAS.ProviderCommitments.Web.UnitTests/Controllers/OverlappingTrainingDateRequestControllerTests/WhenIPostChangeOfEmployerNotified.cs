using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIPostChangeOfEmployerNotified
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public void AndWhenUserSelectsToViewAllCohorts()
        {
            _fixture.SetupChangeOfEmployerNotified(NextAction.ViewAllCohorts);
            _fixture.VerifyUserRedirectedTo("Review");
        }

        [Test]
        public void AndWhenUserSelectsToViewDashBoard()
        {
            _fixture.SetupChangeOfEmployerNotified(NextAction.ViewDashBoard);
            _fixture.VerifyUserRedirectedToUrl();
        }
    }
}