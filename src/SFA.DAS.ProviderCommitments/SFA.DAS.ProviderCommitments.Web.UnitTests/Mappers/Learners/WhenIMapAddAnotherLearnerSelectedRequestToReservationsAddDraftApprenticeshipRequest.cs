using System;
using AutoFixture.NUnit3;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class WhenIMapAddAnotherLearnerSelectedRequestToReservationsAddDraftApprenticeshipRequest
{
    [Test, MoqAutoData]
    public async Task Then_result_properties_are_mapped_from_source_and_cache(
        Guid reservationId,
        int epaPrice,
        GetLearnerSelectedResponse response,
        AddAnotherLearnerSelectedRequest source,
        [Frozen] Mock<ICacheStorageService> cacheService,
        [Frozen] Mock<IOuterApiService> outerApiService,
        [Greedy] AddAnotherDraftApprenticeshipRequestFromLearnerSelectedRequestMapper sut)
    {
        // Arrange
        var cacheItem = new AddAnotherApprenticeshipCacheItem(source.CacheKey)
        {
            ReservationId = reservationId,
            EndPointAssessmentPrice = epaPrice
        };
        AddAnotherApprenticeshipCacheItem savedCacheItem = null;

        cacheService
            .Setup(x => x.RetrieveFromCache<AddAnotherApprenticeshipCacheItem>(source.CacheKey))
            .ReturnsAsync(cacheItem)
            .Verifiable();
        cacheService
            .Setup(x => x.SaveToCache(source.CacheKey, It.IsAny<AddAnotherApprenticeshipCacheItem>(), 1))
            .Callback<Guid, AddAnotherApprenticeshipCacheItem, int>((_, item, _) => savedCacheItem = item)
            .Returns(Task.CompletedTask)
            .Verifiable();

        outerApiService
            .Setup(x => x.GetLearnerSelected(source.ProviderId, source.LearnerDataId))
            .ReturnsAsync(response)
            .Verifiable();

        // Act
        var result = await sut.Map(source);

        // Assert
        result.Should().NotBeNull();
        result.CacheKey.Should().Be(source.CacheKey);
        result.ProviderId.Should().Be(source.ProviderId);
        result.ReservationId.Should().Be(cacheItem.ReservationId);
        result.CohortReference.Should().Be(source.CohortReference);

        savedCacheItem.Should().NotBeNull();
        savedCacheItem.FirstName.Should().Be(response.FirstName);
        savedCacheItem.LastName.Should().Be(response.LastName);
        savedCacheItem.Email.Should().Be(response.Email);
        savedCacheItem.DateOfBirth.Should().Be(response.Dob);
        savedCacheItem.StartDate.Should().Be(response.StartDate);
        savedCacheItem.EndDate.Should().Be(response.PlannedEndDate);
        savedCacheItem.Uln.Should().Be(response.Uln.ToString());
        savedCacheItem.LearnerDataId.Should().Be(source.LearnerDataId);
        savedCacheItem.EndPointAssessmentPrice.Should().Be(response.EpaoPrice);
        savedCacheItem.TrainingPrice.Should().Be(response.TrainingPrice);
        savedCacheItem.CourseCode.Should().Be(response.TrainingCode);
        savedCacheItem.Cost.Should().Be(response.TrainingPrice + response.EpaoPrice);
        savedCacheItem.DeliveryModel.Should().Be(response.IsFlexiJob ? DeliveryModel.FlexiJobAgency : DeliveryModel.Regular);

        outerApiService.Verify();
        outerApiService.VerifyNoOtherCalls();
        cacheService.Verify();
        cacheService.VerifyNoOtherCalls();
    }
}
