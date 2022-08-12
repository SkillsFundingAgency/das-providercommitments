using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIGetOverlappingTrainingDateOptions
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
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
    }
}
