using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class PriceViewModelMapperTests
    {
        private PriceViewModelMapper _mapper;
        private PriceRequest _source;
        private Func<Task<PriceViewModel>> _act;
        private Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
        private GetApprenticeshipResponse _getApprenticeshipApiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<PriceRequest>().Without(x=>x.Price).Create();

            _getApprenticeshipApiResponse = new GetApprenticeshipResponse {EmployerName = "TestName"};

            _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClientMock
                .Setup(x => x.GetApprenticeship(_source.ApprenticeshipId, default(CancellationToken)))
                .ReturnsAsync(_getApprenticeshipApiResponse);
                
            _mapper = new PriceViewModelMapper(_commitmentsApiClientMock.Object);

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
        public async Task ThenLegalEntityNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_getApprenticeshipApiResponse.EmployerName, result.LegalEntityName);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartDate, result.StartDate);
        }

        [TestCase(null)]
        [TestCase(345)]
        public async Task ThenEditModeSetWhenPriceHasAValue(int? price)
        {
            _source.Price = price;
            var result = await _act();
            Assert.AreEqual(price.HasValue, result.InEditMode);
        }
    }
}
