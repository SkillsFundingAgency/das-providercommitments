using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmRequestMapperTests
    {
        private ConfirmRequestMapper _mapper;
        private PriceViewModel _source;
        private Func<Task<ConfirmRequest>> _act;
        private Mock<ICacheStorageService> _cacheStorage;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<PriceViewModel>();

            _cacheStorage = new Mock<ICacheStorageService>();

            _mapper = new ConfirmRequestMapper(Mock.Of<ILogger<ConfirmRequestMapper>>(), _cacheStorage.Object);

            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenProviderIdMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public void ThenThrowsExceptionWhenNewPriceIsNull()
        {
            _source.Price = null;
            Assert.ThrowsAsync<InvalidOperationException>( () => _mapper.Map(TestHelper.Clone(_source)));
        }
    }
}
