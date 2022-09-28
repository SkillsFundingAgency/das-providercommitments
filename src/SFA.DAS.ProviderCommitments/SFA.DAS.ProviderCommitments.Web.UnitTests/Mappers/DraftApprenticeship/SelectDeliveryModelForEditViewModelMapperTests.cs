using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship
{
    [TestFixture]
    public class SelectDeliveryModelForEditViewModelMapperTests
    {
        private SelectDeliveryModelForEditViewModelMapper _mapper;
        private Mock<IOuterApiClient> _apiClient;
        private Mock<ITempDataStorageService> _tempDataStorageService;
        private DraftApprenticeshipRequest _request;
        private GetEditDraftApprenticeshipSelectDeliveryModelResponse _apiResponse;
        private readonly Fixture _fixture = new Fixture();
        private EditDraftApprenticeshipViewModel _cacheModel;

        [SetUp]
        public void Setup()
        {
            _request = _fixture.Create<DraftApprenticeshipRequest>();
            _apiResponse = _fixture.Create<GetEditDraftApprenticeshipSelectDeliveryModelResponse>();
            _cacheModel = new EditDraftApprenticeshipViewModel(new DateTime(2000, 1, 1), null, null, null);

            _tempDataStorageService = new Mock<ITempDataStorageService>();
            _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditDraftApprenticeshipViewModel>())
                .Returns(_cacheModel);

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetEditDraftApprenticeshipSelectDeliveryModelResponse>(It.Is<GetEditDraftApprenticeshipSelectDeliveryModelRequest>(r =>
                    r.DraftApprenticeshipId == _request.DraftApprenticeshipId
                    && r.CohortId == _request.CohortId
                    && r.CourseCode == _cacheModel.CourseCode
                    && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_apiResponse);

            _mapper = new SelectDeliveryModelForEditViewModelMapper(_apiClient.Object, _tempDataStorageService.Object);
        }

        [Test]
        public async Task EmployerName_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.EmployerName, result.LegalEntityName);
        }

        [Test]
        public async Task DeliveryModel_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task DeliveryModels_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.DeliveryModels, result.DeliveryModels);
        }
    }
}
