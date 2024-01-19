using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIGetDraftApprenticeshipOverlapOptions
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public async Task AndWhenIGetDraftApprenticeshipOverlapOptions_CorrectViewModelIsReturned()
        {
            await _fixture.GetDraftApprenticeshipOverlapOptions();
            _fixture.VerifyDraftApprenticeshipOverlapOptionsViewReturned();
        }

        [Test]
        public async Task AndWhenIGetDraftApprenticeshipOverlapOptions_ModelIsMapped()
        {
            await _fixture
                .GetDraftApprenticeshipOverlapOptions();

            _fixture.VerifyWhenGettingOverlappingTrainingDate_ModelIsMapped();
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToSendOverlapEmailToEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmailSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.OverlappingTrainingDateRequestController.Actions.EmployerNotified);
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.CohortController.Actions.Details, ControllerConstants.CohortController.Name);
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToAddApprenticeshipLater()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.CompleteActionLater).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.CohortController.Actions.Review, ControllerConstants.CohortController.Name);
        }

        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Completed, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Live, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Stopped, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Paused, true)]
        public async Task ThenEnableStopRequestEmailIsMappedCorrectly(CommitmentsV2.Types.ApprenticeshipStatus apprenticeshipStatus, bool sendEmail)
        {
            await _fixture
           .SetApprenticeshipStatus(apprenticeshipStatus)
           .GetDraftApprenticeshipOverlapOptions();

            _fixture.VerifyEnableEmployerRequestEmail(sendEmail);
        }
    }
}