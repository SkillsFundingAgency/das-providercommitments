using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class ConfirmEmployerDeclaredStandardsRequestToModelTests
    {
        private ConfirmEmployerDeclaredStandardsRequestToModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private ConfirmEmployerViewModel _request;
        private GetConfirmEmployerDeclaredStandardsResponse _apiResponse;
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public async Task HasNoDeclaredStandards_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.HasNoDeclaredStandards, result.HasNoDeclaredStandards);
        }

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<ConfirmEmployerViewModel>();
            _apiResponse = _fixture.Create<GetConfirmEmployerDeclaredStandardsResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetConfirmEmployerDeclaredStandardsResponse>(It.Is<GetConfirmEmployerDeclaredStandardsRequest>(r =>
                    r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new ConfirmEmployerDeclaredStandardsRequestToModelMapper(_apiClient.Object);
        }
    }
}
