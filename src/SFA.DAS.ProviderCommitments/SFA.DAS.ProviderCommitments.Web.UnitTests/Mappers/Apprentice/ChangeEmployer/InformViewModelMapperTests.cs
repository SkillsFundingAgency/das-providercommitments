using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.ChangeEmployer
{
    [TestFixture]
    public class InformViewModelMapperTests
    {
        private InformViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private ChangeEmployerInformRequest _request;
        private GetInformResponse _apiResponse;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<ChangeEmployerInformRequest>();
            _apiResponse = _fixture.Create<GetInformResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetInformResponse>(It.Is<GetInformRequest>(r =>
                    r.ApprenticeshipId == _request.ApprenticeshipId
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);


            _mapper = new InformViewModelMapper(_apiClient.Object);
        }

        [Test]
        public async Task LegalEntityName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.LegalEntityName, result.LegalEntityName);
        }
    }
}
