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
        private DetailsViewModelMapper _mapper;
        private DetailsRequest _source;
        private GetApprenticeshipResponse _apiResponse;
        private Func<Task<DetailsViewModel>> _act;
        private Mock<IEncodingService> _encodingService;
        private string _cohortReference;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Create<DetailsRequest>();
            _apiResponse = fixture.Create<GetApprenticeshipResponse>();


            _cohortReference = fixture.Create<string>();
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference))
                .Returns(_cohortReference);

            var apiClient = new Mock<ICommitmentsApiClient>();
            apiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apiResponse);

            //apiClient.Setup(x => x.GetLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

            _mapper = new DetailsViewModelMapper(apiClient.Object, _encodingService.Object);
            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenFullNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.FirstName + " " + _apiResponse.LastName, result.ApprenticeName);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EmployerName, result.Employer);
        }

        [Test]
        public async Task ThenReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_cohortReference, result.Reference);
        }
    }
}
