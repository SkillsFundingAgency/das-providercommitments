using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.ChangeEmployer
{
    [TestFixture]
    public class SelectDeliveryModelRequestMapperTests
    {
        private SelectDeliveryModelRequestMapper _mapper;
        private ConfirmEmployerViewModel _request;
        private readonly Fixture _fixture = new Fixture();
        private Mock<ICacheStorageService> _cacheStorage;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<ConfirmEmployerViewModel>();

            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x => x.SaveToCache(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Returns(() => Task.CompletedTask);

            _mapper = new SelectDeliveryModelRequestMapper( _cacheStorage.Object);
        }

        [Test]
        public async Task CacheItem_Is_Updated()
        {
            var result = await _mapper.Map(_request);
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<string>(key => key == result.CacheKey.ToString()),
                    It.Is<ChangeEmployerCacheItem>(c => c.Key == result.CacheKey && c.AccountLegalEntityId == _request.EmployerAccountLegalEntityId),
                    It.IsAny<int>()),
                Times.Once);
        }
    }
}
