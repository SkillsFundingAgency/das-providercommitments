using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetDraftApprenticeshipOverlapOptionsChangeEmployer
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public void AndWhenGetDraftApprenticeshipOverlapOptionsChangeEmployerEndpointIsCalled_CorrectViewModelIsReturned()
        {
            _fixture.GetDraftApprenticeshipOverlapOptionsChangeEmployer();
            _fixture.VerifyDraftApprenticeshipOverlapOptionsViewReturned();
        }
    }
}