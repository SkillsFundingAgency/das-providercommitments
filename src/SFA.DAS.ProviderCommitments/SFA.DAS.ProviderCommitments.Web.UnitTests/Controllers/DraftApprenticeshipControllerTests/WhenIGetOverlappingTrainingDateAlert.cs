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