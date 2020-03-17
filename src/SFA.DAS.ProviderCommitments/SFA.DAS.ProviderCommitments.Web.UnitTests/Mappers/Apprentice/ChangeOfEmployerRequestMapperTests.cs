using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ChangeOfEmployerRequestMapperTests
    {
        private ChangeOfEmployerRequestMapper _mapper;
        private PriceViewModel _source;
        private Func<Task<ChangeOfEmployerRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<PriceViewModel>();

            _mapper = new ChangeOfEmployerRequestMapper(Mock.Of<ILogger<ChangeOfEmployerRequestMapper>>());

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
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenNewStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartDate, result.StartDate);
        }

        [Test]
        public async Task ThenNewPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Price.Value, result.Price);
        }

        [Test]
        public void ThenThrowsExceptionWhenNewPriceIsNull()
        {
            _source.Price = null;
            Assert.ThrowsAsync<InvalidOperationException>( () => _mapper.Map(TestHelper.Clone(_source)));
        }
    }
}
