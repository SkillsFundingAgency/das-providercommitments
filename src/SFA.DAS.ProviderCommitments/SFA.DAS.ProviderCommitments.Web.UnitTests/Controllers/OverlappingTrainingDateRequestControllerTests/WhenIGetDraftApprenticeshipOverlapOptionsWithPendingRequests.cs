using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

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

        [Test]
        public void AndWhenIGetOverlapRequests_FeatureToggleServiceIsCalled()
        {
            _fixture.GetDraftApprenticeshipOverlapOptionsWithPendingRequest();
            _fixture.VerifyfeatureTogglesServiceToGetPendingRequestsIsCalled();
        }

        [Test]
        public void AndWhenIGetDraftApprenticeshipOverlapOptions_ModelIsMapped()
        {
            _fixture.SetOverlappingTrainingDateRequestFeatureToggle(true).GetDraftApprenticeshipOverlapOptionsWithPendingRequest();
            _fixture.VerifyWhenGettingOverlapPendingRequests_ModelIsMapped(true);
        }

    }
}