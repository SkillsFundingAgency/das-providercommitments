using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsSortRequestMapperTests
{
    private Mock<ICacheStorageService> _cacheStorage;
    private SelectMultipleLearnerRecordsSortRequestMapper _mapper;

    private SelectMultipleLearnerRecordsSortRequest _source;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _source = fixture.Create<SelectMultipleLearnerRecordsSortRequest>();
        _cacheItem = fixture.Create<SelectMultipleLearnerRecordsCacheItem>();

        _cacheStorage = new Mock<ICacheStorageService>(MockBehavior.Strict);

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _cacheStorage
            .Setup(x => x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1))
            .Returns(Task.CompletedTask);

        _mapper = new SelectMultipleLearnerRecordsSortRequestMapper(_cacheStorage.Object);
    }

    [Test]
    public async Task Map_WhenSortFieldMatches_AndReverseSortWasFalse_TogglesReverseSortToTrue()
    {
        _cacheItem.SortField = "FirstName";
        _cacheItem.ReverseSort = false;

        _source.SortField = "FirstName";

        var result = await _mapper.Map(_source);

        _cacheItem.SortField.Should().Be("FirstName");
        _cacheItem.ReverseSort.Should().BeTrue();

        _cacheStorage.Verify(x =>
            x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value),
            Times.Once);

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);
    }

    [Test]
    public async Task Map_WhenSortFieldMatches_AndReverseSortWasTrue_SetsReverseSortToFalse()
    {
        _cacheItem.SortField = "FirstName";
        _cacheItem.ReverseSort = true;

        _source.SortField = "FirstName";

        await _mapper.Map(_source);

        _cacheItem.SortField.Should().Be("FirstName");
        _cacheItem.ReverseSort.Should().BeFalse();

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);
    }

    [Test]
    public async Task Map_WhenSortFieldDiffers_SetsReverseSortToFalse()
    {
        _cacheItem.SortField = "FirstName";
        _cacheItem.ReverseSort = true;

        _source.SortField = "Uln";

        await _mapper.Map(_source);

        _cacheItem.SortField.Should().Be("Uln");
        _cacheItem.ReverseSort.Should().BeFalse();

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);
    }    

    [Test]
    public async Task Map_ReturnsRequestWithMatchingProviderId_AndCacheKey()
    {
        _cacheItem.ProviderId = 12345;
        _source.SortField = "FirstName";

        var result = await _mapper.Map(_source);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);
    }
}