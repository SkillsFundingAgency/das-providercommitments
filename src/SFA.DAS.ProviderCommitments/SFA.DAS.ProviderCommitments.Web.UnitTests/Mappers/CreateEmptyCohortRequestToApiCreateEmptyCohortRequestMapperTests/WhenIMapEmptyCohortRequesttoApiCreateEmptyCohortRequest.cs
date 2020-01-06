using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapperTests
{
    public class WhenIMapEmptyCohortRequesttoApiCreateEmptyCohortRequest
    {
        private CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper _mapper;
        private CreateEmptyCohortRequest _source;
        private Func<Task<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>> _act;
        private CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse _accountLegalEntityResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<CreateEmptyCohortRequest>();

            _accountLegalEntityResponse = fixture.Create<CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse>();
            var icommitmentApiClient = new Mock<ICommitmentsApiClient>();
            icommitmentApiClient.Setup(x => x.GetLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

            _mapper = new CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper(icommitmentApiClient.Object);

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenAccountIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountLegalEntityResponse.AccountId, result.AccountId);
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
