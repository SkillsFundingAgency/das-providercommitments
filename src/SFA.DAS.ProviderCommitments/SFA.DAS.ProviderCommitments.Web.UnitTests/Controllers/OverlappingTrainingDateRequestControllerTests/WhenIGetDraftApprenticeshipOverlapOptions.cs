using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

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
        public void AndWhenIGetDraftApprenticeshipOverlapOptions_CorrectViewModelIsReturned()
        {
            _fixture.GetDraftApprenticeshipOverlapOptions();
            _fixture.VerifyDraftApprenticeshipOverlapOptionsViewReturned();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapOptions_FeatureToggleServiceIsCalled()
        {
            _fixture.GetDraftApprenticeshipOverlapOptions();
            _fixture.VerifyfeatureTogglesServiceToGetOverlappingTrainingDateIsCalled();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapOptions_ModelIsMapped()
        {
            _fixture.SetOverlappingTrainingDateRequestFeatureToggle(true).GetDraftApprenticeshipOverlapOptions();
            _fixture.VerifyWhenGettingOverlappingTrainingDate_ModelIsMapped(true);
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToSendOverlapEmailToEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmailSent();
            _fixture.VerifyUserRedirectedTo("Details");
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer).DraftApprenticeshipOverlapOptions();
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo("Details");
        }
    }
}
