using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.ChangeEmployer
{
    [TestFixture]
    public class SelectDeliveryModelViewModelMapperTests
    {
        private SelectDeliveryModelViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ICacheStorageService> _cacheStorage;
        private SelectDeliveryModelRequest _request;
        private GetSelectDeliveryModelResponse _apiResponse;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<SelectDeliveryModelRequest>();
            _apiResponse = _fixture.Create<GetSelectDeliveryModelResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetSelectDeliveryModelResponse>(It.Is<GetSelectDeliveryModelRequest>(r =>
                    r.ApprenticeshipId == _request.ApprenticeshipId
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _cacheStorage = new Mock<ICacheStorageService>();

            _mapper = new SelectDeliveryModelViewModelMapper(_apiClient.Object, _cacheStorage.Object);
        }

        [Test]
        public async Task LegalEntityName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.LegalEntityName, result.LegalEntityName);
        }

        [Test]
        public async Task DeliveryModels_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.DeliveryModels, result.DeliveryModels);
        }
    }
}
