using System;
using AutoFixture;
using Moq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class SelectDeliveryModelViewModelMapperTests
    {
        private SelectDeliveryModelViewModelMapper _mapper;

        private Mock<IOuterApiClient> _apiClient;
        private Mock<ICacheStorageService> _cacheStorageService;
        private CreateCohortWithDraftApprenticeshipRequest _request;
        private GetAddDraftApprenticeshipDeliveryModelResponse _apiResponse;
        private readonly Fixture _fixture = new Fixture();
        private CreateCohortCacheModel _cacheModel;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetAddDraftApprenticeshipDeliveryModelResponse>();
            _cacheModel = new CreateCohortCacheModel(Guid.NewGuid());

            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateCohortCacheModel>(It.IsAny<Guid>()))
                .ReturnsAsync(_cacheModel);

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDeliveryModelResponse>(It.Is<GetAddDraftApprenticeshipDeliveryModelRequest>(r =>
                    r.AccountLegalEntityId == _request.AccountLegalEntityId
                    && r.CourseCode == _cacheModel.CourseCode
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new SelectDeliveryModelViewModelMapper(_apiClient.Object, _cacheStorageService.Object);
        }

        [Test]
        public async Task EmployerName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.EmployerName, result.EmployerName);
        }

        [Test]
        public async Task DeliveryModels_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.DeliveryModels, result.DeliveryModels);
        }


        [Test]
        public void When_Only_One_Delivery_Model_Is_Available_It_Is_Saved_To_Cache()
        {
            throw new NotImplementedException();
        }

    }
}
