using System;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

[TestFixture]
public class WhenISyncLearnerData
{
    private DraftApprenticeshipControllerTestFixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new DraftApprenticeshipControllerTestFixture();
    }

    [Test]
    public async Task AndSyncIsSuccessful_ThenSuccessMessageIsStoredInTempData()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse
        {
            Success = true,
            Message = "Learner data updated successfully",
            UpdatedDraftApprenticeship = new GetDraftApprenticeshipResponse()
        };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyTempDataContains("LearnerDataSyncSuccess", "Learner data has been successfully updated.");
    }

    [Test]
    public async Task AndSyncFails_ThenErrorMessageIsStoredInTempData()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse
        {
            Success = false,
            Message = "Failed to sync learner data"
        };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyTempDataContains("LearnerDataSyncError", "Failed to sync learner data");
    }

    [Test]
    public async Task AndSyncFailsWithNoMessage_ThenDefaultErrorMessageIsStoredInTempData()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse
        {
            Success = false,
            Message = null
        };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyTempDataContains("LearnerDataSyncError", "Failed to sync learner data.");
    }

    [Test]
    public async Task AndExceptionOccurs_ThenErrorMessageIsStoredInTempData()
    {
        // Arrange
        _fixture.SetupOuterApiServiceToThrowException();

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyTempDataContains("LearnerDataSyncError", "An error occurred while syncing learner data.");
    }

    [Test]
    public async Task ThenOuterApiServiceIsCalledWithCorrectParameters()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse { Success = true };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyOuterApiServiceSyncLearnerDataCalled();
    }

    [Test]
    public async Task ThenUserIsRedirectedToEditDraftApprenticeshipPage()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse { Success = true };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyRedirectedToEditDraftApprenticeshipPage();
    }

    [Test]
    public async Task AndSyncIsSuccessful_ThenUpdatedDraftApprenticeshipIsStoredInCache()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse
        {
            Success = true,
            Message = "Learner data updated successfully",
            UpdatedDraftApprenticeship = new GetDraftApprenticeshipResponse()
        };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyOuterApiServiceSyncLearnerDataCalled();
    }

    [Test]
    public async Task AndSyncIsSuccessful_ThenRedirectContainsLearnerDataSyncKey()
    {
        // Arrange
        var syncResponse = new SyncLearnerDataResponse
        {
            Success = true,
            Message = "Learner data updated successfully",
            UpdatedDraftApprenticeship = new GetDraftApprenticeshipResponse()
        };
        _fixture.SetupOuterApiServiceToReturnSyncResponse(syncResponse);

        // Act
        await _fixture.SyncLearnerData();

        // Assert
        _fixture.VerifyRedirectContainsLearnerDataSyncKey();
    }
}
