using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapChangeEmployerRedirectRequestToSelectMultipleLearnerRecordsRequest
{
    [Test]
    public async Task Then_RetrievesCacheItem_FromCache_Once_UsingSourceCacheKey()
    {
        var fixture = new ChangeEmployerRedirectRequestMapperFixture();

        await fixture.Act();

        fixture.Verify_RetrieveFromCache_CalledOnce_WithSourceKey();
    }

    [Test]
    public async Task Then_UpdatesCacheItem_AndSavesToCache_Once_WithMappedValues_AndTtl()
    {
        var fixture = new ChangeEmployerRedirectRequestMapperFixture();

        await fixture.Act();

        fixture.Verify_SaveToCache_CalledOnce();
        fixture.Assert_CacheItem_UpdatedCorrectly();
        fixture.Assert_SaveToCache_UsesCacheItemKey_AndTtlIs1();
    }

    [Test]
    public async Task Then_ReturnsRequest_WithProviderId_AndCacheKeyFromCacheItem()
    {
        var fixture = new ChangeEmployerRedirectRequestMapperFixture();

        var result = await fixture.Act();

        fixture.Assert_Result_MappedCorrectly(result);
    }

    private class ChangeEmployerRedirectRequestMapperFixture
    {
        private readonly Mock<ICacheStorageService> _cacheStorageMock;
        private readonly ChangeEmployerRedirectRequestMapper _sut;

        private ChangeEmployerRedirectRequest _source;
        private SelectMultipleLearnerRecordsCacheItem _cacheItem;

        private string _savedKey;
        private SelectMultipleLearnerRecordsCacheItem _savedItem;
        private int _savedTtl;

        public ChangeEmployerRedirectRequestMapperFixture()
        {
            _source = new ChangeEmployerRedirectRequest
            {
                ProviderId = 123,
                CacheKey = Guid.NewGuid(),
                EmployerAccountLegalEntityPublicHashedId = "DSFF23",
                EmployerAccountName = "TestAccountName"
            };

            _cacheItem = new SelectMultipleLearnerRecordsCacheItem(Guid.NewGuid())
            {
                ProviderId = 999, // will be overwritten
                EmployerAccountLegalEntityPublicHashedId = "OLD",
                UseLearnerData = false,
                EmployerAccountName = "OLDNAME"
            };

            _cacheStorageMock = new Mock<ICacheStorageService>();

            _cacheStorageMock
                .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey!.Value))
                .ReturnsAsync(_cacheItem);

            _cacheStorageMock
                .Setup(x => x.SaveToCache(
                    It.IsAny<string>(),
                    It.IsAny<SelectMultipleLearnerRecordsCacheItem>(),
                    It.IsAny<int>()))
                .Callback<string, SelectMultipleLearnerRecordsCacheItem, int>((key, item, ttl) =>
                {
                    _savedKey = key;
                    _savedItem = item;
                    _savedTtl = ttl;
                })
                .Returns(Task.CompletedTask);

            _sut = new ChangeEmployerRedirectRequestMapper(_cacheStorageMock.Object);
        }

        public async Task<SelectMultipleLearnerRecordsRequest> Act() => await _sut.Map(_source);

        public void Verify_RetrieveFromCache_CalledOnce_WithSourceKey()
        {
            _cacheStorageMock.Verify(
                x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_source.CacheKey!.Value),
                Times.Once);
        }

        public void Verify_SaveToCache_CalledOnce()
        {
            _cacheStorageMock.Verify(x => x.SaveToCache(
                It.IsAny<string>(),
                It.IsAny<SelectMultipleLearnerRecordsCacheItem>(),
                It.IsAny<int>()), Times.Once);
        }

        public void Assert_CacheItem_UpdatedCorrectly()
        {
            _savedItem.Should().NotBeNull();

            _savedItem.ProviderId.Should().Be(_source.ProviderId);
            _savedItem.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
            _savedItem.EmployerAccountName.Should().Be(_source.EmployerAccountName);
        }

        public void Assert_SaveToCache_UsesCacheItemKey_AndTtlIs1()
        {
            _savedItem.Should().NotBeNull();
            _savedKey.Should().NotBeNull();

            _savedKey.Should().Be(_savedItem.Key.ToString());
            _savedTtl.Should().Be(1);
        }

        public void Assert_Result_MappedCorrectly(SelectMultipleLearnerRecordsRequest result)
        {
            result.ProviderId.Should().Be(_source.ProviderId);
            result.CacheKey.Should().Be(_cacheItem.CacheKey);
        }
    }
}
