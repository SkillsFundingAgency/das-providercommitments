using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapSelectEmployerRedirectRequestToSelectMultipleLearnerRecords
{
    [Test]
    public async Task Then_SavesCacheItem_ToCache_Once_WithMappedValues_AndTtl()
    {
        var fixture = new SelectEmployerRedirectRequestMapperFixture();

        await fixture.Act();

        fixture.Verify_SaveToCache_CalledOnce();
        fixture.Assert_SavedCacheItem_MappedCorrectly();
        fixture.Assert_SaveToCache_UsesCacheItemKey_AndTtlIs1();
    }

    [Test]
    public async Task Then_ReturnsRequest_WithProviderId_AndCacheKeyFromSavedItem()
    {
        var fixture = new SelectEmployerRedirectRequestMapperFixture();

        var result = await fixture.Act();

        fixture.Assert_Result_MappedCorrectly(result);
    }

    private class SelectEmployerRedirectRequestMapperFixture
    {
        private readonly Mock<ICacheStorageService> _cacheStorageMock;
        private readonly SelectEmployerRedirectRequestMapper _sut;
        private SelectEmployerRedirectRequest _source;

        private string _savedKey;
        private SelectMultipleLearnerRecordsCacheItem _savedItem;
        private int _savedTtl;

        public SelectEmployerRedirectRequestMapperFixture()
        {
            _source = new SelectEmployerRedirectRequest
            {
                ProviderId = 123,
                EmployerAccountLegalEntityPublicHashedId = "DSFF23",
                UseLearnerData = true,
                EmployerAccountName = "TestAccountName"
            };

            _cacheStorageMock = new Mock<ICacheStorageService>();

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

            _sut = new SelectEmployerRedirectRequestMapper(_cacheStorageMock.Object);
        }

        public async Task<SelectMultipleLearnerRecordsRequest> Act() => await _sut.Map(_source);

        public void Verify_SaveToCache_CalledOnce()
        {
            _cacheStorageMock.Verify(x => x.SaveToCache(
                It.IsAny<string>(),
                It.IsAny<SelectMultipleLearnerRecordsCacheItem>(),
                It.IsAny<int>()), Times.Once);
        }

        public void Assert_SavedCacheItem_MappedCorrectly()
        {
            _savedItem.Should().NotBeNull();

            _savedItem.ProviderId.Should().Be(_source.ProviderId);
            _savedItem.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
            _savedItem.UseLearnerData.Should().Be(_source.UseLearnerData);
            _savedItem.EmployerAccountName.Should().Be(_source.EmployerAccountName);
        }

        public void Assert_SaveToCache_UsesCacheItemKey_AndTtlIs1()
        {
            _savedItem.Should().NotBeNull();
            _savedKey.Should().NotBeNull();

            _savedItem.Key.Should().NotBe(Guid.Empty);
            _savedKey.Should().Be(_savedItem.Key.ToString());
            _savedTtl.Should().Be(1);
        }

        public void Assert_Result_MappedCorrectly(SelectMultipleLearnerRecordsRequest result)
        {
            _savedItem.Should().NotBeNull();

            result.ProviderId.Should().Be(_source.ProviderId);
            result.CacheKey.Should().Be(_savedItem!.CacheKey);
        }
    }
}
