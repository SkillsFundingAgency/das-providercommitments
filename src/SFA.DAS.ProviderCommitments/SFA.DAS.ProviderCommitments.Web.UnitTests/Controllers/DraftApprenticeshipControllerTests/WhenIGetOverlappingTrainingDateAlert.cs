using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIGetOverlappingTrainingDateAlert
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapAlert_CorrectViewModelIsReturned()
        {
            _fixture.GetDraftApprenticeshipOverlapAlert();
            _fixture.VerifyDraftApprenticeshipOverlapAlertViewReturned();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapAlert_FeatureToggleServiceIsCalled()
        {
            _fixture.GetDraftApprenticeshipOverlapAlert();
            _fixture.VerifyfeatureTogglesServiceToGetOverlappingTrainingDateIsCalled();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapAlert_ModelIsMapped()
        {
            _fixture.SetOverlappingTrainingDateRequestFeatureToggle(true).GetDraftApprenticeshipOverlapAlert();
            _fixture.VerifyWhenGettingOverlappingTrainingDateAlert_ModelIsMapped(true);
        }
    }
}
