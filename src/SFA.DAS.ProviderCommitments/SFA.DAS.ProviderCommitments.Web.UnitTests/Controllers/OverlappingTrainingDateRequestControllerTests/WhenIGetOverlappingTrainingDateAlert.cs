namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetOverlappingTrainingDateAlert
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();

            _fixture.SetupPeekTempDraftApprenticeship();
            _fixture.SetupPeekTempEditDraftApprenticeship();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapAlert_CorrectViewModelIsReturned()
        {
            _fixture.GetDraftApprenticeshipOverlapAlert();
            _fixture.VerifyDraftApprenticeshipOverlapAlertViewReturned();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapAlert_PeekStoredEditDraftApprenticeshipStateIsCalled()
        {
            _fixture.GetDraftApprenticeshipOverlapAlert();
            _fixture.VerifyPeekStoredEditDraftApprenticeshipStateIsCalled();
        }
    }
}