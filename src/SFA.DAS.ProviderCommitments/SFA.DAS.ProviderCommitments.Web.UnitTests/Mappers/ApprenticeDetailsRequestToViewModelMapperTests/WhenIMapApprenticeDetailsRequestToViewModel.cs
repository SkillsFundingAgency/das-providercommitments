using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeDetailsRequestToViewModelMapperTests
{
    [TestFixture]
    public class WhenIMapApprenticeDetailsRequestToViewModel
    {
        private ApprenticeDetailsRequestToViewModelMapper _mapper;
        private ApprenticeDetailsRequest _source; 
        private Func<Task<ApprenticeDetailsViewModel>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<ApprenticeDetailsRequest>();
            var icommitmentApiClient = new Mock<ICommitmentsApiClient>();
            //icommitmentApiClient.Setup(x => x.GetLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

            _mapper = new ApprenticeDetailsRequestToViewModelMapper(icommitmentApiClient.Object);
            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        //[Test]
        //public async Task ThenNameIsMappedCorrectly()
        //{
        //    var result = await _act();
        //    Assert.AreEqual(_accountLegalEntityResponse.LegalEntityName, result.EmployerAccountLegalEntityName);
        //}

        //[Test]
        //public async Task ThenProviderIdMappedCorrectly()
        //{
        //    var result = await _act();
        //    Assert.AreEqual(_source.ProviderId, result.ProviderId);
        //}

        //[Test]
        //public async Task ThenAccountLegalEntityIdIsNotMapped()
        //{
        //    var result = await _act();
        //    Assert.AreEqual(0, result.AccountLegalEntityId);
        //}
    }
}
