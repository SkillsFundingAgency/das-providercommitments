namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetOverlapOptionsForChangeEmployer
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public async Task AndWhenGetDraftApprenticeshipOverlapOptionsChangeEmployerEndpointIsCalled_CorrectViewModelIsReturned()
        {
            await _fixture.GetOverlapOptionsForChangeEmployer();
            _fixture.VerifyOverlapOptionsForChangeEmployerViewModelViewReturned();
        }

        [Test]
        public async Task AndWhenGetDraftApprenticeshipOverlapOptionsChangeEmployerEndpointIsCalled_NotWithdrawnFromIlrPreviousApprenticeship_CorrectViewModelIsReturned()
        {
            await _fixture.SetupWithdrawnStatusCode(null).GetOverlapOptionsForChangeEmployer();
            _fixture.VerifyOverlapOptionsForChangeEmployerViewModelViewReturnedWithNoWithdrawnReasonCode();
        }
    }
}