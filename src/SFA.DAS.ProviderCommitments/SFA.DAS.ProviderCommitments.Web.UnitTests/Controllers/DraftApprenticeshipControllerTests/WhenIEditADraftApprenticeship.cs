using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIEditADraftApprenticeship
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task ShouldReturnEditDraftApprenticeshipView()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        public async Task ShouldSetProviderIdOnViewModel()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice().SetupProviderIdOnEditRequest(123);
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelHasProviderIdSet();
        }

        [Test]
        public async Task ShouldPassCohortIdAndDraftApprenticeshipIdToGetDraftApprenticeshipOnProviderCommitmentsService()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyGetDraftApprenticeshipReceivesCorrectParameters();
        }

        [Test]
        public async Task IfCalledWithAnInvalidRequestShouldGetBadResponseReturned()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyWeGetABadRequestResponse();
        }

        [Test]
        public async Task AndWhenSavingTheDraftApprenticeIsSuccessful()
        {
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsItShouldReturnTheViewWithModelAndErrors()
        {

            _fixture.SetupUpdatingToThrowCommitmentsApiException();
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyEditViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task AndWhenSavingFailsDueToModelBindingItShouldReturnTheViewWithModelAndErrors()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyEditViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FrameworkCourseAreOnlyRequestedWhenCohortisNotFundedByTransfer(bool isFundedByTransfer)
        {
            _fixture.SetupCohortTransferFundedStatus(isFundedByTransfer);
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyWhetherFrameworkCourseWereRequested(!isFundedByTransfer);
        }

    }
}