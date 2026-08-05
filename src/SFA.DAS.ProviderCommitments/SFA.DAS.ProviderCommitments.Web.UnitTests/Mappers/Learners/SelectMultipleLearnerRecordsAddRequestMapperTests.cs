using System.Linq;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsAddRequestMapperTests
{
    private Mock<ICacheStorageService> _cacheStorage;
    private SelectMultipleLearnerRecordsAddRequestMapper _mapper;

    private SelectMultipleLearnerRecordsAddRequest _source;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _source = fixture.Create<SelectMultipleLearnerRecordsAddRequest>();
        _cacheItem = fixture.Create<SelectMultipleLearnerRecordsCacheItem>();
        _cacheItem.SelectedLearners = [];

        _cacheStorage = new Mock<ICacheStorageService>();

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _cacheStorage
            .Setup(x => x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1))
            .Returns(Task.CompletedTask);

        _mapper = new SelectMultipleLearnerRecordsAddRequestMapper(_cacheStorage.Object);
    }

    [Test]
    public async Task Map_WhenLearnerIsNotAlreadySelected_AddsLearner_SavesToCache_AndReturnsRequest()
    {
        var result = await _mapper.Map(_source);

        _cacheItem.SelectedLearners.Should().ContainSingle();
        _cacheItem.SelectedLearners.Single().Should().BeEquivalentTo(_source.Learner);

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
    public async Task Map_WhenLearnerIsAlreadySelected_DoesNotAddDuplicate_DoesNotSavesToCache_AndReturnsRequest()
    {
        _cacheItem.SelectedLearners.Add(_source.Learner);

        var result = await _mapper.Map(_source);

        _cacheItem.SelectedLearners.Should().ContainSingle();
        _cacheItem.SelectedLearners.Single().Id.Should().Be(_source.Learner.Id);

        _cacheStorage.Verify(x =>
            x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value),
            Times.Once);

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Never);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);
    }
}