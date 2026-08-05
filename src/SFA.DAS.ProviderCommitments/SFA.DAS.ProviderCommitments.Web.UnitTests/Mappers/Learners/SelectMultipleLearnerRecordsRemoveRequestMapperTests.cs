using System.Linq;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsRemoveRequestMapperTests
{
    private Mock<ICacheStorageService> _cacheStorage;
    private SelectMultipleLearnerRecordsRemoveRequestMapper _mapper;

    private SelectMultipleLearnerRecordsRemoveRequest _source;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _source = fixture.Create<SelectMultipleLearnerRecordsRemoveRequest>();
        _cacheItem = fixture.Create<SelectMultipleLearnerRecordsCacheItem>();
        _cacheItem.SelectedLearners = [];

        _cacheStorage = new Mock<ICacheStorageService>();

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _cacheStorage
            .Setup(x => x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1))
            .Returns(Task.CompletedTask);

        _mapper = new SelectMultipleLearnerRecordsRemoveRequestMapper(_cacheStorage.Object);
    }

    [Test]
    public async Task Map_WhenLearnerExists_RemovesLearner_SavesToCache_AndReturnsRequest()
    {
        var fixture = new Fixture();

        var learnerToRemove = fixture.Build<LearnerSummary>()
            .With(x => x.Id, _source.LearnerId)
            .Create();

        var learnerToKeep = fixture.Build<LearnerSummary>()
            .With(x => x.Id, _source.LearnerId + 1)
            .Create();

        _cacheItem.SelectedLearners.Add(learnerToRemove);
        _cacheItem.SelectedLearners.Add(learnerToKeep);

        var result = await _mapper.Map(_source);

        _cacheItem.SelectedLearners.Should().ContainSingle();
        _cacheItem.SelectedLearners.Single().Id.Should().Be(learnerToKeep.Id);
        _cacheItem.SelectedLearners.Should().NotContain(x => x.Id == _source.LearnerId);

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
    public async Task Map_WhenLearnerDoesNotExist_DoesNotRemoveLearner_DoesNotSaveToCache_AndReturnsRequest()
    {
        var fixture = new Fixture();

        var existingLearner = fixture.Build<LearnerSummary>()
            .With(x => x.Id, _source.LearnerId + 1)
            .Create();

        _cacheItem.SelectedLearners.Add(existingLearner);

        var result = await _mapper.Map(_source);

        _cacheItem.SelectedLearners.Should().ContainSingle();
        _cacheItem.SelectedLearners.Single().Id.Should().Be(existingLearner.Id);

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