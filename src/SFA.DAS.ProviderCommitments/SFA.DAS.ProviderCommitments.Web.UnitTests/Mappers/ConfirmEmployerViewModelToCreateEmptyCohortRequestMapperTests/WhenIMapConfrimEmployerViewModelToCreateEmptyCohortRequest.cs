using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ConfirmEmployerViewModelToCreateEmptyCohortRequestMapperTests
{
    public class WhenIMapConfrimEmployerViewModelToCreateEmptyCohortRequest
    {
        private ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper _mapper;
        private ConfirmEmployerViewModel _source;
        private Func<Task<CreateEmptyCohortRequest>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<ConfirmEmployerViewModel>();
            _mapper = new ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper();

           _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenProviderIdMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.AccountLegalEntityId, result.AccountLegalEntityId);
        }
    }
}
