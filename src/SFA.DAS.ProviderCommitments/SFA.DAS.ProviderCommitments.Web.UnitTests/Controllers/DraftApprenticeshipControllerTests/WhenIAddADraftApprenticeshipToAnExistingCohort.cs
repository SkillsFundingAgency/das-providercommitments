using System.Threading.Tasks;
using NUnit.Framework;

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
            _fixture.VerifyViewHasCohortButWithoutAReservationId();
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
            _fixture.VerifyViewHasCohortWithAReservationId()
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
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsItShouldReturnTheViewWithModelAndErrors()
        {

            _fixture.SetupSaveToThrowCommitmentsApiException();
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task AndWhenSavingFailsduetoModelBindingItShouldReturnTheViewWithModelAndErrors()
        {

            _fixture.SetupModelStateToBeInvalid();
            await _fixture.PostDraftApprenticeship();
            _fixture.VerifyViewWasReturnedAndHasErrors()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }
    }
}