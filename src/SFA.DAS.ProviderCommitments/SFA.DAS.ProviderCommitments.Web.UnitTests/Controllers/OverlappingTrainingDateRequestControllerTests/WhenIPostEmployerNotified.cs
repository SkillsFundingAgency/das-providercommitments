using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIPostEmployerNotified
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
            _fixture.SetupEmployerNotified(NextAction.ViewAllCohorts);
            _fixture.VerifyUserRedirectedTo("Review");
        }

        [Test]
        public void AndWhenUserSelectsToAddAnotherApprentice()
        {
            _fixture.SetupEmployerNotified(NextAction.AddAnotherApprentice);
            _fixture.VerifyUserRedirectedTo("Details");
        }

        [Test]
        public void AndWhenUserSelectsToViewDashBoard()
        {
            _fixture.SetupEmployerNotified(NextAction.ViewDashBoard);
            _fixture.VerifyUserRedirectedToUrl();
        }
    }
}