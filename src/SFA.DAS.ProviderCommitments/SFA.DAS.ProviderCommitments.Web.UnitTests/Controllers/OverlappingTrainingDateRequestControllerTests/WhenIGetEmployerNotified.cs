using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetEmployerNotified
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public void AndWhenGetEmployerNotifiedEndPointIsCalled_CorrectViewModelIsReturned()
        {
            _fixture.GetEmployerNotified();
            _fixture.VerifyEmployerNotifiedViewReturned();
        }
    }
}