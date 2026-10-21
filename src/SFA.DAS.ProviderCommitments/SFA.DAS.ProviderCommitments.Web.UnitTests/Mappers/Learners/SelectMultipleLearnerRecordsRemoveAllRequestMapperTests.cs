using System.Linq;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsRemoveAllRequestMapperTests
{
    private Mock<ICacheStorageService> _cacheStorage;
    private SelectMultipleLearnerRecordsRemoveAllRequestMapper _mapper;

    private SelectMultipleLearnerRecordsRemoveAllRequest _source;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _source = fixture.Create<SelectMultipleLearnerRecordsRemoveAllRequest>();
        _cacheItem = fixture.Create<SelectMultipleLearnerRecordsCacheItem>();
        _cacheItem.SelectedLearners = [];

        _cacheStorage = new Mock<ICacheStorageService>();

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _cacheStorage
            .Setup(x => x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1))
            .Returns(Task.CompletedTask);

        _mapper = new SelectMultipleLearnerRecordsRemoveAllRequestMapper(_cacheStorage.Object);
    }

    [Test]
    public async Task Map_WhenSelectedLearnersExists_RemovesAllLearners_SavesToCache_AndReturnsRequest()
    {
        var fixture = new Fixture();

        var learnerOne = fixture.Build<LearnerSummary>()
            .With(x => x.Id, 1)
            .Create();

        var learnerTwo = fixture.Build<LearnerSummary>()
            .With(x => x.Id, 2)
            .Create();

        _cacheItem.SelectedLearners.Add(learnerOne);
        _cacheItem.SelectedLearners.Add(learnerTwo);

        var result = await _mapper.Map(_source);

        _cacheItem.SelectedLearners.Should().BeEmpty();
        _cacheItem.SelectedLearners.Should().NotContain(x => x.Id == learnerOne.Id);
        _cacheItem.SelectedLearners.Should().NotContain(x => x.Id == learnerTwo.Id);

        _cacheStorage.Verify(x =>
            x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey.Value),
            Times.Once);

        _cacheStorage.Verify(x =>
            x.SaveToCache(_cacheItem.Key.ToString(), _cacheItem, 1),
            Times.Once);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CacheKey.Should().Be(_source.CacheKey);
    }
}