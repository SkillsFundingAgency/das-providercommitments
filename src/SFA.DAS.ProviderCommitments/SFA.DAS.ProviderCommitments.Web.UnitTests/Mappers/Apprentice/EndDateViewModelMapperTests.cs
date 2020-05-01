using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class EndDateViewModelMapperTests
    {
        private EndDateViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new EndDateViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [TestCase(null)]
        [TestCase("022020")]
        public async Task ThenStartDateIsMapped(string startDate)
        {
            _fixture.Request.StartDate = startDate;
            var result = await _fixture.Act();

            Assert.AreEqual(startDate, result.StartDate);
        }

        [TestCase(null)]
        [TestCase(234)]
        public async Task ThenPriceIsMapped(int? price)
        {
            _fixture.Request.Price = price;
            var result = await _fixture.Act();

            Assert.AreEqual(price, result.Price);
        }

        [TestCase("")]
        [TestCase("042019")]
        public async Task ThenEndDateIsMapped(string endDate)
        {
            _fixture.Request.EndDate = endDate;
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.EndDate, result.EndDate.MonthYear);
        }

        [TestCase(null)]
        [TestCase(234)]
        public async Task ThenEditModeIsOnWhenAPriceHasAValue(int? price)
        {
            _fixture.Request.Price = price;
            var result = await _fixture.Act();

            Assert.AreEqual(price.HasValue, result.InEditMode);
        }
    }

    class EndDateViewModelMapperFixture
    {
        private readonly EndDateViewModelMapper _sut;

        public EndDateRequest Request { get; }

        public EndDateViewModelMapperFixture()
        {
            Request = new EndDateRequest
            {
                ApprenticeshipHashedId = "SF45G54",
                ApprenticeshipId = 234,
                ProviderId = 645621,
                EmployerAccountLegalEntityPublicHashedId = "GD35SD35",
                StartDate = "122019"
            };
            _sut = new EndDateViewModelMapper();
        }

        public Task<EndDateViewModel> Act() => _sut.Map(Request);
    }
}