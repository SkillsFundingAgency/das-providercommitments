using System;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

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
            _fixture.SetupCommitmentsApiToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        public async Task AndThereIsTempDataStored_ShouldReturnTempDataEditDraftApprenticeshipView()
        {
            _fixture.SetupTempDraftApprenticeship();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        public async Task AndWhenSavingApprenticeshipStartedBeforeMandatoryRpl()
        {
            _fixture
                .SetApprenticeshipStarting("2022-08-01")
                .SetupCommitmentsApiToReturnADraftApprentice();

            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectToRecognisePriorLearningPage();
        }

        [Test]
        public async Task AndWhenEditingCourse()
        {
            await _fixture.PostToEditDraftApprenticeship("Edit");
            _fixture.VerifyRedirectedToSelectForEditCoursePage()
                .VerifyRedirectIncludesCohortIdAndDraftApprenticeshipId();
        }

        [Test]
        public async Task AndWhenEditingDeliveryModel()
        {
            await _fixture.PostToEditDraftApprenticeship(changeDeliveryModel: "Edit");
            _fixture.VerifyRedirectedToSelectDeliveryForEditModelPage();
        }

        [Test]
        public void AndWhenSavingFailsDueToValidationItShouldThrowCommitmentsApiModelException()
        {
            _fixture.SetupUpdatingToThrowCommitmentsApiException();
            var action =() => _fixture.PostToEditDraftApprenticeship();
            action.Should().ThrowAsync<CommitmentsApiModelException>();
        }

        [Test]
        public async Task AndWhenSavesRedirectsToSelectOptionsViewIfHasOptions()
        {
            _fixture
                .SetApprenticeshipStarting("2022-08-01")
                .SetupCommitmentsApiToReturnADraftApprentice();

            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectToRecognisePriorLearningPage();
        }

        [Test]
        public async Task AndWhenThereIsStartDateOverlap()
        {
            await _fixture.SetupStartDateOverlap(true, false).SetupEditDraftApprenticeshipViewModelForStartDateOverlap().PostToEditDraftApprenticeship();
            _fixture.VerifyUserRedirectedTo("DraftApprenticeshipOverlapAlert");
        }

        [Test]
        public async Task AndWhenLearnerDataSyncKeyIsProvided_ThenViewIsReturned()
        {
            // Arrange
            var cacheKey = Guid.NewGuid().ToString();
            var updatedDraftApprenticeship = new GetDraftApprenticeshipResponse
            {
                FirstName = "Updated",
                LastName = "Name"
            };
            
            _fixture.SetupCommitmentsApiToReturnADraftApprentice();

            // Act
            await _fixture.EditDraftApprenticeshipWithLearnerDataSyncKey(cacheKey);

            // Assert
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }
    }
}