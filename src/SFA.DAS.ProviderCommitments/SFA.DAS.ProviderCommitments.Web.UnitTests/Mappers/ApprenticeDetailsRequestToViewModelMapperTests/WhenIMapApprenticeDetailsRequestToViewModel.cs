using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeDetailsRequestToViewModelMapperTests
{
    [TestFixture]
    public class WhenIMapApprenticeDetailsRequestToViewModel
    {
        private WhenIMapApprenticeDetailsRequestToViewModelFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenIMapApprenticeDetailsRequestToViewModelFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.Source.ApprenticeshipHashedId, _fixture.Result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenFullNameIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.FirstName + " " + _fixture.ApiResponse.LastName, _fixture.Result.ApprenticeName);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.EmployerName, _fixture.Result.Employer);
        }

        [Test]
        public async Task ThenReferenceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.CohortReference, _fixture.Result.Reference);
        }

        [Test]
        public async Task ThenStatusIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Status, _fixture.Result.Status);
        }

        [Test]
        public async Task ThenStopDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.StopDate, _fixture.Result.StopDate);
        }

        [Test]
        public async Task ThenAgreementIdIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.AgreementId, _fixture.Result.AgreementId);
        }

        public class WhenIMapApprenticeDetailsRequestToViewModelFixture
        {
            private readonly DetailsViewModelMapper _mapper;
            public DetailsRequest Source { get; }
            public DetailsViewModel Result { get; private set; }
            public GetApprenticeshipResponse ApiResponse { get; }
            
            private readonly Mock<IEncodingService> _encodingService;
            public string CohortReference { get; }
            public string AgreementId { get; }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture()
            {
                var fixture = new Fixture();
                Source = fixture.Create<DetailsRequest>();
                ApiResponse = fixture.Create<GetApprenticeshipResponse>();
                CohortReference = fixture.Create<string>();
                AgreementId = fixture.Create<string>();

                _encodingService = new Mock<IEncodingService>();
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns(CohortReference);
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns(AgreementId);

                var apiClient = new Mock<ICommitmentsApiClient>();
                apiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ApiResponse);

                _mapper = new DetailsViewModelMapper(apiClient.Object, _encodingService.Object);
            }

            public async Task<WhenIMapApprenticeDetailsRequestToViewModelFixture> Map()
            {
                Result = await _mapper.Map(Source);
                return this;
            }
        }
    }
}
