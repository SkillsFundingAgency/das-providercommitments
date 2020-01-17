using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ConfirmEmployerRequestToViewModelMapperTests
{
    public class WhenIMapConfrimEmployerRequestToConfirmEmployerViewModel
    {
        private ConfirmEmployerRequestToViewModelMapper _mapper;
        private ConfirmEmployerRequest _source;
        private Func<Task<ConfirmEmployerViewModel>> _act;
        private CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse _accountLegalEntityResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _accountLegalEntityResponse = fixture.Create<CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse>();
            _source = fixture.Create<ConfirmEmployerRequest>();
            var icommitmentApiClient = new Mock<ICommitmentsApiClient>();
            icommitmentApiClient.Setup(x => x.GetLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

            _mapper = new ConfirmEmployerRequestToViewModelMapper(icommitmentApiClient.Object);

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenEmployerAccountNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountLegalEntityResponse.AccountName, result.EmployerAccountName);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_accountLegalEntityResponse.LegalEntityName, result.EmployerAccountLegalEntityName);
        }

        [Test]
        public async Task ThenProviderIdMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsNotMapped()
        {
            var result = await _act();
            Assert.AreEqual(0, result.AccountLegalEntityId);
        }
    }
}
