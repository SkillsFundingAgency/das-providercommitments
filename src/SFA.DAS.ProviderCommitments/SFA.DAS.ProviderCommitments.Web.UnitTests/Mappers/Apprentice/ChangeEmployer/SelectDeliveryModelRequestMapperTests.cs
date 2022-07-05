using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
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
        private Mock<IEncodingService> _encodingService;
        private long _accountLegalEntityId;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<ConfirmEmployerViewModel>();

            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x => x.SaveToCache(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Returns(() => Task.CompletedTask);

            _accountLegalEntityId = _fixture.Create<long>();
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x =>
                    x.Decode(It.Is<string>(s => s == _request.EmployerAccountLegalEntityPublicHashedId),
                        It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
                .Returns(_accountLegalEntityId);

            _mapper = new SelectDeliveryModelRequestMapper( _cacheStorage.Object, _encodingService.Object);
        }

        [Test]
        public async Task CacheItem_Is_Updated()
        {
            var result = await _mapper.Map(_request);
            _cacheStorage.Verify(x => x.SaveToCache(It.Is<string>(key => key == result.CacheKey.ToString()),
                    It.Is<ChangeEmployerCacheItem>(c => c.Key == result.CacheKey && c.AccountLegalEntityId == _accountLegalEntityId),
                    It.IsAny<int>()),
                Times.Once);
        }
    }
}
