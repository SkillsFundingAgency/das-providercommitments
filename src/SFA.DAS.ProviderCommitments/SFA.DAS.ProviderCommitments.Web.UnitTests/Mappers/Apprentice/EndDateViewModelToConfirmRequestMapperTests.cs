using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class EndDateViewModelToConfirmRequestMapperTests
    {
        private EndDateViewModelToConfirmRequestMapper _mapper;
        private EndDateViewModel _source;
        private Func<Task<ConfirmRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<EndDateViewModel>()
                .With(x => x.EndDate, new MonthYearModel("042020"))
                .With(x => x.StartDate, "012019")
                .Without(x => x.EmploymentEndDate)
                .Create();

            _mapper = new EndDateViewModelToConfirmRequestMapper(Mock.Of<ILogger<EndDateViewModelToConfirmRequestMapper>>());

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
    }
}
