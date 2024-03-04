namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetDraftApprenticeshipOverlapOptionsWithPendingRequests
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public void AndWhenGetOverlapRequestsEndPointIsCalled_CorrectViewModelIsReturned()
        {
            _fixture.GetDraftApprenticeshipOverlapOptionsWithPendingRequest();
            _fixture.VerifyOverlapRequestsViewReturned();
        }
    }
}