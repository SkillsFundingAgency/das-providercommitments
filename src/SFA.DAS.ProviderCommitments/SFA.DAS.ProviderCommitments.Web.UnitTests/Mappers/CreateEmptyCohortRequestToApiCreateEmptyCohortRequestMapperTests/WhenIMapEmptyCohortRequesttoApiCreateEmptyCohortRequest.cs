using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapperTests
{
    public class WhenIMapEmptyCohortRequesttoApiCreateEmptyCohortRequest
    {
        private CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper _mapper;
        private CreateEmptyCohortRequest _source;
        private Func<Task<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>> _act;
      
        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
           
            _source = fixture.Create<CreateEmptyCohortRequest>();
            _mapper = new CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper();

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenAccountIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.AccountId, result.AccountId);
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
