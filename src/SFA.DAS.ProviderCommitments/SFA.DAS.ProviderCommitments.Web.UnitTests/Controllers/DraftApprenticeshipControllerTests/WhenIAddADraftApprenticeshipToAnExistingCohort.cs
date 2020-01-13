using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIAddADraftApprenticeshipToAnExistingCohort
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task IfCalledDirectlyFromProvideApprenticeshipServiceItShouldReturnAddDraftApprenticeshipViewWithCohortButWithoutAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyAddViewHasCohortButWithoutAReservationId();
        }

        [Test]
        public async Task IfCalledDirectlyFromProvideApprenticeshipServiceWithAnInvalidRequestShouldGetBadResponse()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyWeGetABadRequestResponse();
        }

        [Test]
        public async Task IfCalledViaReservationsItShouldReturnAddDraftApprenticeshipViewWithCohortAndWithAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyAddViewHasCohortWithAReservationId()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task IfCalledViaReservationsWithAnInvalidRequestShouldGetBadResponse()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.AddDraftApprenticeshipWithoutReservation();
            _fixture.VerifyWeGetABadRequestResponse();
        }


        [Test]
        public async Task AndWhenSavingTheApprenticeToCohortIsSuccessful()
        {
            await _fixture.PostToAddDraftApprenticeship();
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsBecauseOfModelValidationItShouldThrowCommitmentApiModelException()
        {
            _fixture.SetupAddingToThrowCommitmentsApiException();
            Assert.ThrowsAsync<CommitmentsApiModelException>(async ()=>  await _fixture.PostToAddDraftApprenticeship());
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