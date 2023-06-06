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
    public class HasDeclaredStandardsRequestToViewModelMapperTests
    {
        private HasDeclaredStandardsRequestToViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private ConfirmEmployerViewModel _request;
        private GetHasDeclaredStandardsResponse _apiResponse;
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
            _apiResponse = _fixture.Create<GetHasDeclaredStandardsResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetHasDeclaredStandardsResponse>(It.Is<GetHasDeclaredStandardsRequest>(r =>
                    r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new HasDeclaredStandardsRequestToViewModelMapper(_apiClient.Object);
        }
    }
}
