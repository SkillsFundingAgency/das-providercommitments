using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsFilterRequestMapperTests
{
    private Mock<ICacheStorageService> _cacheStorage;
    private SelectMultipleLearnerRecordsFilterRequestMapper _mapper;

    private SelectMultipleLearnerRecordsFilterRequest _source;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _source = fixture.Create<SelectMultipleLearnerRecordsFilterRequest>();
        _cacheItem = fixture.Create<SelectMultipleLearnerRecordsCacheItem>();

        _cacheStorage = new Mock<ICacheStorageService>();

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _cacheStorage
            .Setup(x => x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1))
            .Returns(Task.CompletedTask);

        _mapper = new SelectMultipleLearnerRecordsFilterRequestMapper(_cacheStorage.Object);
    }

    [Test]
    public async Task Map_WhenClearFilterTrue_ClearsValues_SavesToCache_AndReturnsRequest()
    {
        var expectedYear = DateTime.UtcNow.Year.ToString();

        _source.ClearFilter = true;

        _cacheItem.SearchTerm = "old";
        _cacheItem.StartMonth = "3";
        _cacheItem.StartYear = "2020";

        var result = await _mapper.Map(_source);

        _cacheStorage.Verify(x =>
            x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value),
            Times.Once);

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);

        _cacheItem.SearchTerm.Should().Be("");
        _cacheItem.StartMonth.Should().BeNull();
        _cacheItem.StartYear.Should().Be(expectedYear);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);
    }

    [Test]
    public async Task Map_WhenClearFilterFalse_CopiesValuesFromSource_SavesToCache_AndReturnsRequest()
    {
        _source.ClearFilter = false;
        _source.SearchTerm = "abc";
        _source.StartMonth = 5;
        _source.StartYear = 2024;

        var result = await _mapper.Map(_source);

        _cacheItem.SearchTerm.Should().Be("abc");
        _cacheItem.StartMonth.Should().Be("5");
        _cacheItem.StartYear.Should().Be("2024");

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);

        _cacheStorage.Verify(x =>
            x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value),
            Times.Once);
    }
}