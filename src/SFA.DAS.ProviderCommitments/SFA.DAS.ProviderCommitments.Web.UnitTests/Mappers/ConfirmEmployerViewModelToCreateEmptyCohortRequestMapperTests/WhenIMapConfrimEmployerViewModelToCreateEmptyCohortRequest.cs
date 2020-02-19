using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ConfirmEmployerViewModelToCreateEmptyCohortRequestMapperTests
{
    public class WhenIMapConfrimEmployerViewModelToCreateEmptyCohortRequest
    {
        private ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper _mapper;
        private ConfirmEmployerViewModel _source;
        private Func<Task<CreateEmptyCohortRequest>> _act;
        private Mock<ICommitmentsApiClient> _commitmentApiClient;
        private AccountLegalEntityResponse _accountlegalEntityResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();
             _accountlegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();

            _source = fixture.Create<ConfirmEmployerViewModel>();
            _commitmentApiClient.Setup(x => x.GetLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountlegalEntityResponse);

            _mapper = new ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper(_commitmentApiClient.Object);

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

        [Test]
        public async Task ThenAccountIdMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountlegalEntityResponse.AccountId, result.AccountId);
        }
    }
}
