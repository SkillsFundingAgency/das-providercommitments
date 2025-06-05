using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIAddADraftApprenticeshipToAnExistingCohort
    {
        private DraftApprenticeshipControllerTestFixture _fixture;
        private const string DateBeforeRplRequired = "2022-07-31";
        private const string DateAfterRplRequired = "2022-08-01";

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task IfCalledViaReservationsItShouldReturnAddDraftApprenticeshipViewWithCohortAndWithAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyMappingFromReservationAddRequestIsCalled()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public void AndWhenSavingFailsBecauseOfModelValidationItShouldThrowCommitmentApiModelException()
        {
            _fixture.SetupAddingToThrowCommitmentsApiException();
            var action = () =>  _fixture.PostToAddDraftApprenticeship();
            action.Should().ThrowAsync<CommitmentsApiModelException>();
        }

        [TestCase(true, ApprenticeshipEmployerType.Levy, false)]
        [TestCase(false, ApprenticeshipEmployerType.Levy, true)]
        [TestCase(true, ApprenticeshipEmployerType.NonLevy, false)]
        [TestCase(false, ApprenticeshipEmployerType.NonLevy, false)]
        public async Task AndWhenGettingCourseListVerifyIfFrameworksAreIncludedOrNot(bool isTransfer, ApprenticeshipEmployerType levyStatus, bool areFrameworksIncluded)
        {
            _fixture.SetupLevyStatus(levyStatus).SetupCohortFundedByTransfer(isTransfer);
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyWhetherFrameworkCourseWereRequested(areFrameworksIncluded);
        }

        [Test]
        public async Task AndWhenGettingCourseListIncludeFrameworksForChangeOfPartyRequests()
        {
            _fixture.SetupLevyStatus(ApprenticeshipEmployerType.NonLevy)
                .SetupCohortFundedByTransfer(false)
                .SetCohortWithChangeOfParty(true);

            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyWhetherFrameworkCourseWereRequested(true);
        }

        [Test]
        public async Task AndWhenReturningToPageAfterChangingTheCourseOrDeliveryModel()
        {
            _fixture.SetupTempDraftApprenticeship();

            await _fixture.AddDraftApprenticeshipWithReservation();

            _fixture.VerifyViewModelFromTempDataHasDeliveryModelAndCourseValuesSet();
        }

        [Test]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithDeliveryModelToggleWeRedirectToSelectCourse()
        {
            _fixture.SetupUseLearnerData(false);
            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToSelectCoursePage();
        }

        [Test]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithIlrFeatureOnAndUseLearnerDataAsTrueRedirectsToSelectLearner()
        {
            _fixture.SetupUseLearnerData(true);
            _fixture.SetupIlrConfigurationSection(true);
            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToSelectLearnerPage();
        }

        [Test]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithIlrFeatureOnAndUseLearnerDataAsFalseRedirectsToSelectCourse()
        {
            _fixture.SetupUseLearnerData(true);
            _fixture.SetupIlrConfigurationSection(false);
            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToSelectCoursePage();
        }


        [TestCase(true)]
        [TestCase(false)]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithILRFeatureOffRedirectsToSelectCourse(bool useLearnerData)
        {
            _fixture.SetupUseLearnerData(useLearnerData);
            _fixture.SetupIlrConfigurationSection(false);
            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToSelectCoursePage();
        }

        [Test]
        public void AndWhenCallingTheGetReservationIdEndpointRedirectToReservationsPage()
        {
            _fixture.GetReservationId();
            _fixture.VerifyRedirectedToReservationsPage();
        }

        [Test]
        public void AndWhenGoingToSelectHowPageWithUseLearnerDataAsFalseThenRedirectToGetReservationIdEndpoint()
        {
            _fixture.SetupUseLearnerData(false);
            _fixture.GotoSelectHowPage();
            _fixture.VerifyRedirectedToGetReservationIdEndpoint();
        }

        [Test]
        public void AndWhenGoingToSelectHowPageWithUseLearnerDataAsTrueThenReturnSelectHowView()
        {
            _fixture.SetupUseLearnerData(true);
            var result = _fixture.GotoSelectHowPage();
            result.VerifyReturnsSelectHowViewModelWithCorrectValues(); 
        }

        [TestCase(AddAnotherDraftApprenticeshipJourneyOptions.Ilr, true)]
        [TestCase(AddAnotherDraftApprenticeshipJourneyOptions.Manual, false)]
        public async Task AndWhenAddApprenticeshipViaIlrThenRedirectToSelectFromIlrPage(AddAnotherDraftApprenticeshipJourneyOptions option, bool expected)
        {
            _fixture.PostToAddAnotherSelectionMethod(option);
            _fixture.VerifyRedirectedToGetReservationIdEndpoint();
            _fixture.VerifyRouteValueContainsUseLearnerDataAs(expected);
        }

        [Test]
        public void AndWhenCallingTheGetReservationIdEndpointRedirectToReservationsPageWithTransferSender()
        {
            _fixture.GetReservationId("ABCD");
            _fixture.VerifyRedirectedToReservationsPage();
            _fixture.VerifyRedirectedToReservationsPageContainsTransferSenderId();
        }

        [Test]
        public async Task AndWhenApprenticeshipStartsBeforeMandatoryRplThenRedirectToRplQuestion()
        {
            _fixture
                .SetApprenticeshipStarting(DateBeforeRplRequired)
                .SetUpStandardToReturnNoOptions();

            await _fixture.PostToAddDraftApprenticeship();

            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedToRplQuestion();
        }

        [Test]
        public async Task AndWhenApprenticeshipStartDateIsNotSetThenStillRedirectToSetRplQuestion()
        {
            _fixture
                .SetApprenticeshipStarting(null)
                .SetUpStandardToReturnNoOptions();

            await _fixture.PostToAddDraftApprenticeship();

            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedToRplQuestion();
        }

        [Test]
        public async Task AndWhenApprenticeshipStartsBeforeMandatoryRplAndThereAreStandardOptionsThenRedirectToSelectOptions()
        {
            _fixture
                .SetApprenticeshipStarting(DateBeforeRplRequired)
                .SetUpStandardToReturnOptions();

            await _fixture.PostToAddDraftApprenticeship();

            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectToSelectOptionsPage();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task AndWhenApprenticeshipStartsAfterMandatoryRplThenRedirectToRecognitionOfPriorLearning(bool isActualDate)
        {
            _fixture.SetApprenticeshipStarting(DateAfterRplRequired, isActualDate);

            await _fixture.PostToAddDraftApprenticeship();

            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectToRecognisePriorLearningPage();
        }

        [Test]
        public async Task AndSelectCourseIsToBeChangedThenTheUserIsRedirectedToSelectCoursePage()
        {
            await _fixture.PostToAddDraftApprenticeship(changeCourse: "Edit");
            _fixture.VerifyUserRedirectedTo("AddDraftApprenticeshipCourse");
        }

        [Test]
        public async Task AndSelectDeliveryModelIsToBeChangedThenTheUserIsRedirectedToSelectDeliveryModelPage()
        {
            await _fixture.PostToAddDraftApprenticeship(changeDeliveryModel: "Edit");
            _fixture.VerifyUserRedirectedTo("SelectDeliveryModel");
        }

        [Test]
        public async Task AndPilotStatusIsToBeChangedThenTheUserIsRedirectedToSelectPilotStatusPage()
        {
            await _fixture.PostToAddDraftApprenticeship(changePilotStatus: "Edit");
            _fixture.VerifyUserRedirectedTo("ChoosePilotStatusForDraftChange");
        }

        [Test]
        public async Task AndWhenThereIsStartDateOverlap()
        {
            await _fixture.SetupStartDateOverlap(true, false).SetupAddDraftApprenticeshipViewModelForStartDateOverlap().PostToAddDraftApprenticeship();
            _fixture.VerifyUserRedirectedTo("DraftApprenticeshipOverlapAlert");
        }
    }
}