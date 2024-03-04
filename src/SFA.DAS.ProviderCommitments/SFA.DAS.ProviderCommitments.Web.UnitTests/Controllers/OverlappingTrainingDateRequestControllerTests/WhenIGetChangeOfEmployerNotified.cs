namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetChangeOfChangeOfEmployerNotified
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public void AndWhenGetChangeOfEmployerNotifiedEndPointIsCalled_CorrectViewModelIsReturned()
        {
            _fixture.GetChangeOfEmployerNotified();
            _fixture.VerifyChangeOfEmployerNotifiedViewReturned();
        }
    }
}