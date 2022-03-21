﻿using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;

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
        public async Task IfCalledViaReservationsItShouldReturnAddDraftApprenticeshipViewWithCohortAndWithAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyMappingFromReservationAddRequestIsCalled()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task AndWhenSavingTheApprenticeToCohortIsSuccessfulAndRedirectedToCohortIfNoStandardOptions()
        {
            _fixture.SetUpStandardToReturnNoOptions()
                .SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToAddDraftApprenticeship();
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsBecauseOfModelValidationItShouldThrowCommitmentApiModelException()
        {
            _fixture.SetupAddingToThrowCommitmentsApiException();
            Assert.ThrowsAsync<CommitmentsApiModelException>(async () => await _fixture.PostToAddDraftApprenticeship());
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
        public async Task AndWhenReturningToPageAfterChnagingTheCourseOrDeliveryModel()
        {
            _fixture.SetupTempDraftApprenticeship();
            
            await _fixture.AddDraftApprenticeshipWithReservation();

            _fixture.VerifyViewModelFromTempDataHasDeliveryModelAndCourseValuesSet();
        }

        [Test]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithDeliveryModelToggleWeRedirectToSelectCourse()
        {
            _fixture.SetupDeliveryModelFeatureToggle();

            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToSelectCoursePage();
        }

        [Test]
        public async Task AndWhenCallingTheAddNewDraftApprenticeshipEndpointWithoutDeliveryModelToggleWeRedirectToAddDraftApprenticeshipPage()
        {
            await _fixture.AddNewDraftApprenticeshipWithReservation();
            _fixture.VerifyRedirectedToAddDraftApprenticeshipDetails();
        }

        [Test]
        public async Task ThenIfThereAreOptionsThenRedirectToSelectOptions()
        {
            _fixture
                .SetUpStandardToReturnOptions()
                .SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToAddDraftApprenticeship();
            
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectToSelectOptionsPage();
        }

        [Test]
        public async Task AndSelectCourseIsToBeChangedThenTheUserIsRedirectedToSelectCoursePage()
        {
            await _fixture.PostToAddDraftApprenticeship(changeCourse: "Edit");
            _fixture.VerifyUserRedirectedTo("SelectCourse");
        }

        [Test]
        public async Task AndSelectDeliveryModelIsToBeChangedThenTheUserIsRedirectedToSelectDeliveryModelPage()
        {
            await _fixture.PostToAddDraftApprenticeship(changeDeliveryModel: "Edit");
            _fixture.VerifyUserRedirectedTo("SelectDeliveryModel");
        }
    }
}