using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi;

public class CachedOuterApiServiceTests
{
    private Mock<ICacheStorageService> _mockCacheStorageService;
    private Mock<IOuterApiService> _mockOuterApiService;

    [SetUp]
    public void SetUp()
    {
        _mockCacheStorageService = new Mock<ICacheStorageService>();
        _mockOuterApiService = new Mock<IOuterApiService>();
    }

    [Test, MoqAutoData]
    public async Task Then_HasPermission_Result_Is_Retrieved_From_OuterApiService_And_Stored_To_Cache_When_Null(
        long ukprn,
        long accountLegalEntityId,
        string operation,
        bool result)
    {
        var cacheKey = $"{nameof(CachedOuterApiService.HasPermission)}.{ukprn}.{accountLegalEntityId}.{operation}";

        _mockCacheStorageService
            .Setup(x => x.RetrieveFromCache<bool?>(cacheKey))
            .ReturnsAsync((bool?)null);

        _mockOuterApiService
            .Setup(x => x.HasPermission(ukprn, accountLegalEntityId, operation))
            .ReturnsAsync(result);

        var sut = new CachedOuterApiService(_mockCacheStorageService.Object, _mockOuterApiService.Object);
        var actual = await sut.HasPermission(ukprn, accountLegalEntityId, operation);

        actual.Should().Be(result);

        _mockCacheStorageService.Verify(x => x.RetrieveFromCache<bool?>(cacheKey), Times.Once);
        _mockCacheStorageService.Verify(x => x.SaveToCache(cacheKey, result, TimeSpan.FromMinutes(CachedOuterApiService.CacheExpirationMinutes)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_CanAccessApprenticeship_Result_Is_Retrieved_From_OuterApiService_And_Stored_To_Cache_When_Null(
        Party party,
        long partyId,
        long apprenticeshipId,
        bool result)
    {
        var cacheKey = $"{nameof(CachedOuterApiService.CanAccessApprenticeship)}.{party}.{partyId}.{apprenticeshipId}";

        _mockCacheStorageService
            .Setup(x => x.RetrieveFromCache<bool?>(cacheKey))
            .ReturnsAsync((bool?)null);

        _mockOuterApiService.Setup(x => x.CanAccessApprenticeship(party, partyId, apprenticeshipId)).ReturnsAsync(result);

        var sut = new CachedOuterApiService(_mockCacheStorageService.Object, _mockOuterApiService.Object);
        var actual = await sut.CanAccessApprenticeship(party, partyId, apprenticeshipId);

        actual.Should().Be(result);

        _mockCacheStorageService.Verify(x => x.RetrieveFromCache<bool?>(cacheKey), Times.Once);
        _mockCacheStorageService.Verify(x => x.SaveToCache(cacheKey, result, TimeSpan.FromMinutes(CachedOuterApiService.CacheExpirationMinutes)), Times.Once);
    }
    
    [Test, MoqAutoData]
    public async Task Then_CanAccessCohort_Result_Is_Retrieved_From_OuterApiService_And_Stored_To_Cache_When_Null(
        Party party,
        long partyId,
        long cohortId,
        bool result)
    {
        var cacheKey = $"{nameof(CachedOuterApiService.CanAccessCohort)}.{party}.{partyId}.{cohortId}";

        _mockCacheStorageService
            .Setup(x => x.RetrieveFromCache<bool?>(cacheKey))
            .ReturnsAsync((bool?)null);

        _mockOuterApiService.Setup(x => x.CanAccessCohort(party, partyId, cohortId)).ReturnsAsync(result);

        var sut = new CachedOuterApiService(_mockCacheStorageService.Object, _mockOuterApiService.Object);
        var actual = await sut.CanAccessCohort(party, partyId, cohortId);

        actual.Should().Be(result);

        _mockCacheStorageService.Verify(x => x.RetrieveFromCache<bool?>(cacheKey), Times.Once);
        _mockCacheStorageService.Verify(x => x.SaveToCache(cacheKey, result, TimeSpan.FromMinutes(CachedOuterApiService.CacheExpirationMinutes)), Times.Once);
    }
}