using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmRequestMapperTests
    {
        private ConfirmRequestMapper _mapper;
        private PriceViewModel _source;
        private Func<Task<ConfirmRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<PriceViewModel>();

            _mapper = new ConfirmRequestMapper(Mock.Of<ILogger<ConfirmRequestMapper>>());

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
        public async Task ThenNewEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EndDate, result.EndDate);
        }

        [Test]
        public async Task ThenNewPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Price.Value, result.Price);
        }

        [Test]
        public async Task ThenNewEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentEndDate, result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenNewEnploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmploymentPrice.Value, result.EmploymentPrice);
        }

        [Test]
        public async Task ThenNullEmploymentEndDateIsMappedCorrectly()
        {
            _source.EmploymentEndDate = null;
            var result = await _act();
            Assert.IsNull(result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenNullEnploymentPriceIsMappedCorrectly()
        {
            _source.EmploymentPrice = null;
            var result = await _act();
            Assert.IsNull(result.EmploymentPrice);
        }

        [Test]
        public void ThenThrowsExceptionWhenNewPriceIsNull()
        {
            _source.Price = null;
            Assert.ThrowsAsync<InvalidOperationException>( () => _mapper.Map(TestHelper.Clone(_source)));
        }
    }
}
