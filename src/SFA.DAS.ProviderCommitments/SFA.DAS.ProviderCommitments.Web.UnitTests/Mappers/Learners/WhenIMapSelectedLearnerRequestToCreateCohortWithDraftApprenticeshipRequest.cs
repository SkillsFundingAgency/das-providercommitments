using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class WhenIMapSelectedLearnerRequestToCreateCohortWithDraftApprenticeshipRequest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICacheStorageService> _cacheService = new();
    private readonly Mock<IOuterApiService> _outerApiService = new();
    private CreateCohortWithDraftApprenticeshipRequestFromLearnerSelectedRequestMapper _sut;
    
    [SetUp]
    public void Setup()
    {
        _sut = new CreateCohortWithDraftApprenticeshipRequestFromLearnerSelectedRequestMapper(_cacheService.Object, _outerApiService.Object);
    }
    
    [Test]
    public async Task Then_result_properties_are_mapped_from_source_and_cache()
    {
        // Arrange
        var response = _fixture.Create<GetLearnerSelectedResponse>();
        var source = _fixture.Create<LearnerSelectedRequest>();
        var cacheItem = _fixture.Create<CreateCohortCacheItem>();
        cacheItem.CacheKey = source.CacheKey;
        CreateCohortCacheItem savedCacheItem = null;
        
        _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey)).ReturnsAsync(cacheItem);
        _cacheService
            .Setup(x => x.SaveToCache(source.CacheKey, It.IsAny<CreateCohortCacheItem>(), 1))
            .Callback<Guid, CreateCohortCacheItem, int>((_, item, _) => savedCacheItem = item)
            .Returns(Task.CompletedTask);
        
        _outerApiService.Setup(x => x.GetLearnerSelected(source.ProviderId, source.LearnerDataId)).ReturnsAsync(response);
        
        // Act
        var result = await _sut.Map(source);

        // Assert
        result.Should().NotBeNull();
        result.CacheKey.Should().Be(source.CacheKey);
        result.ProviderId.Should().Be(source.ProviderId);
        result.ReservationId.Should().Be(cacheItem.ReservationId);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(source.EmployerAccountLegalEntityPublicHashedId);
        result.AccountLegalEntityId.Should().Be(source.AccountLegalEntityId);

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
        
        _outerApiService.VerifyAll();
        _outerApiService.VerifyNoOtherCalls();
        _cacheService.VerifyAll();
        _cacheService.VerifyNoOtherCalls();
    }
}
