using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIPostDraftApprenticeshipOverlapOptions
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToSendOverlapEmailToEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmailSent();
            _fixture.VerifyUserRedirectedTo("EmployerNotified");
        }

        [Test]
        public async Task AndWhenExistingDraftApprenticeship_AndUserSelectsToSendOverlapEmailToEmployer_ThenUpdateRecord()
        {
            await _fixture
                .SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest)
                .SetupUpdateDraftApprenticeshipRequestMapper()
                .SetupGetTempEditDraftApprenticeship()
                .WithCohortReference()
                .WithDraftApprenticeshipHashedId()
                .DraftApprenticeshipOverlapOptions();

            _fixture.VerifyExistingDraftApprenticeshipUpdated();
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo("Details");
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToAddApprenticeshipLater()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.CompleteActionLater).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo("Review");
        }
    }
}