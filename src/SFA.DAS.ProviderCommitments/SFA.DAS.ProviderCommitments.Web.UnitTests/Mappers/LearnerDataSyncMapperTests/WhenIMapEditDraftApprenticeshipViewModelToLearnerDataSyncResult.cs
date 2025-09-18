using System;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.LearnerDataSyncMapperTests
{
    [TestFixture]
    public class WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResult
    {
        private WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture();
        }

        [Test]
        public async Task AndOuterApiCallSucceeds_ThenReturnsSuccessResultWithCacheKey()
        {
            // Arrange
            var syncResponse = new SyncLearnerDataResponse
            {
                Success = true,
                Message = "Learner data updated successfully",
                UpdatedDraftApprenticeship = new GetDraftApprenticeshipResponse
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            };
            _fixture.SetupOuterApiServiceToReturnResponse(syncResponse);
            _fixture.SetupCacheStorageService();

            // Act
            var result = await _fixture.Map();

            // Assert
            _fixture.VerifyResultIsSuccess(result);
            _fixture.VerifyOuterApiServiceCalled();
            _fixture.VerifyCacheStorageServiceSaveCalled();
        }

        [Test]
        public async Task AndOuterApiCallFails_ThenReturnsFailureResult()
        {
            // Arrange
            var syncResponse = new SyncLearnerDataResponse
            {
                Success = false,
                Message = "API call failed"
            };
            _fixture.SetupOuterApiServiceToReturnResponse(syncResponse);

            // Act
            var result = await _fixture.Map();

            // Assert
            _fixture.VerifyResultIsFailure(result, "API call failed");
            _fixture.VerifyOuterApiServiceCalled();
            _fixture.VerifyCacheStorageServiceNotCalled();
        }

        [Test]
        public async Task AndOuterApiCallFailsWithNoMessage_ThenReturnsDefaultFailureMessage()
        {
            // Arrange
            var syncResponse = new SyncLearnerDataResponse
            {
                Success = false,
                Message = null
            };
            _fixture.SetupOuterApiServiceToReturnResponse(syncResponse);

            // Act
            var result = await _fixture.Map();

            // Assert
            _fixture.VerifyResultIsFailure(result, "Failed to sync learner data.");
            _fixture.VerifyOuterApiServiceCalled();
            _fixture.VerifyCacheStorageServiceNotCalled();
        }

        [Test]
        public async Task AndOuterApiThrowsException_ThenReturnsExceptionResult()
        {
            // Arrange
            _fixture.SetupOuterApiServiceToThrowException();

            // Act
            var result = await _fixture.Map();

            // Assert
            _fixture.VerifyResultIsFailure(result, "An error occurred while syncing learner data.");
            _fixture.VerifyOuterApiServiceCalled();
            _fixture.VerifyCacheStorageServiceNotCalled();
        }

        [Test]
        public async Task AndOuterApiCallSucceeds_ThenCallsOuterApiWithCorrectParameters()
        {
            // Arrange
            var syncResponse = new SyncLearnerDataResponse { Success = true };
            _fixture.SetupOuterApiServiceToReturnResponse(syncResponse);
            _fixture.SetupCacheStorageService();

            // Act
            await _fixture.Map();

            // Assert
            _fixture.VerifyOuterApiServiceCalledWithCorrectParameters();
        }

        [Test]
        public async Task AndOuterApiCallSucceeds_ThenSavesToCacheWithCorrectParameters()
        {
            // Arrange
            var syncResponse = new SyncLearnerDataResponse 
            { 
                Success = true,
                UpdatedDraftApprenticeship = new GetDraftApprenticeshipResponse()
            };
            _fixture.SetupOuterApiServiceToReturnResponse(syncResponse);
            _fixture.SetupCacheStorageService();

            // Act
            await _fixture.Map();

            // Assert
            _fixture.VerifyCacheStorageServiceSaveCalledWithCorrectParameters();
        }
    }

    public class WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture
    {
        private readonly Fixture _autoFixture;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly Mock<ICacheStorageService> _cacheStorageService;
        private readonly LearnerDataSyncMapper _mapper;
        private readonly EditDraftApprenticeshipViewModel _viewModel;

        public WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture()
        {
            _autoFixture = new Fixture();
            _outerApiService = new Mock<IOuterApiService>();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _mapper = new LearnerDataSyncMapper(_outerApiService.Object, _cacheStorageService.Object);
            
            _viewModel = new EditDraftApprenticeshipViewModel(
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 1),
                new DateTime(2001, 1, 1),
                new DateTime(2001, 1, 1))
            {
                ProviderId = 123,
                CohortId = 456,
                DraftApprenticeshipId = 789
            };
        }

        public async Task<LearnerDataSyncResult> Map()
        {
            return await _mapper.Map(_viewModel);
        }

        public WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture SetupOuterApiServiceToReturnResponse(SyncLearnerDataResponse response)
        {
            _outerApiService
                .Setup(x => x.SyncLearnerData(_viewModel.ProviderId, _viewModel.CohortId.Value, _viewModel.DraftApprenticeshipId.Value))
                .ReturnsAsync(response);
            return this;
        }

        public WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture SetupOuterApiServiceToThrowException()
        {
            _outerApiService
                .Setup(x => x.SyncLearnerData(_viewModel.ProviderId, _viewModel.CohortId.Value, _viewModel.DraftApprenticeshipId.Value))
                .ThrowsAsync(new Exception("Test exception"));
            return this;
        }

        public WhenIMapEditDraftApprenticeshipViewModelToLearnerDataSyncResultFixture SetupCacheStorageService()
        {
            _cacheStorageService
                .Setup(x => x.SaveToCache(It.IsAny<Guid>(), It.IsAny<object>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            return this;
        }

        public void VerifyResultIsSuccess(LearnerDataSyncResult result)
        {
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Learner data has been successfully updated.");
            result.CacheKey.Should().NotBeNull();
            result.CacheKey.Should().NotBeEmpty();
        }

        public void VerifyResultIsFailure(LearnerDataSyncResult result, string expectedMessage)
        {
            result.Success.Should().BeFalse();
            result.Message.Should().Be(expectedMessage);
            result.CacheKey.Should().BeNull();
        }

        public void VerifyOuterApiServiceCalled()
        {
            _outerApiService.Verify(
                x => x.SyncLearnerData(_viewModel.ProviderId, _viewModel.CohortId.Value, _viewModel.DraftApprenticeshipId.Value), 
                Times.Once);
        }

        public void VerifyOuterApiServiceCalledWithCorrectParameters()
        {
            _outerApiService.Verify(
                x => x.SyncLearnerData(123, 456, 789), 
                Times.Once);
        }

        public void VerifyCacheStorageServiceSaveCalled()
        {
            _cacheStorageService.Verify(
                x => x.SaveToCache(It.IsAny<Guid>(), It.IsAny<object>(), It.IsAny<int>()), 
                Times.Once);
        }

        public void VerifyCacheStorageServiceSaveCalledWithCorrectParameters()
        {
            _cacheStorageService.Verify(
                x => x.SaveToCache(It.IsAny<Guid>(), It.IsAny<GetDraftApprenticeshipResponse>(), 1), 
                Times.Once);
        }

        public void VerifyCacheStorageServiceNotCalled()
        {
            _cacheStorageService.Verify(
                x => x.SaveToCache(It.IsAny<Guid>(), It.IsAny<object>(), It.IsAny<int>()), 
                Times.Never);
        }
    }
}
