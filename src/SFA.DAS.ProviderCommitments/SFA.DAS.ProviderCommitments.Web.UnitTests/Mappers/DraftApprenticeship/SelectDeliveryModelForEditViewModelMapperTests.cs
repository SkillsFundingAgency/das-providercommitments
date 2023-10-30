using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
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
        
        [TestCase(true, DeliveryModel.FlexiJobAgency, true)]
        [TestCase(true, DeliveryModel.PortableFlexiJob, false)]
        [TestCase(false, DeliveryModel.FlexiJobAgency, false)]
        public async Task HasUnavailableFlexiJobAgencyDeliveryModel_Is_Mapped_Correctly(bool hasUnavailableDeliveryModel, CommitmentsV2.Types.DeliveryModel currentDeliveryModel, bool expectedResult)
        {
            _cacheModel.DeliveryModel = currentDeliveryModel;
            _apiResponse.HasUnavailableDeliveryModel = hasUnavailableDeliveryModel;

            var result = await _mapper.Map(_request);
            Assert.AreEqual(expectedResult, result.HasUnavailableFlexiJobAgencyDeliveryModel);
        }

        [TestCase(DeliveryModel.FlexiJobAgency, true, false, true)]
        [TestCase(DeliveryModel.PortableFlexiJob, true, false, false)]
        [TestCase(DeliveryModel.FlexiJobAgency, false, false, false)]
        [TestCase(DeliveryModel.FlexiJobAgency, true, true, false)]
        public async Task ShowFlexiJobAgencyDeliveryModelConfirmation_Is_Mapped_Correctly(DeliveryModel deliveryModel, bool deliveryModelIsUnavailable, bool hasOtherOptions, bool expectShowConfirmation)
        {
            _apiResponse.DeliveryModels.Clear();
            _apiResponse.DeliveryModels.Add(DeliveryModel.Regular);
            _apiResponse.DeliveryModel = deliveryModel;
            _apiResponse.HasUnavailableDeliveryModel = deliveryModelIsUnavailable;

            if (hasOtherOptions)
            {
                _apiResponse.DeliveryModels.Add(DeliveryModel.PortableFlexiJob);
            }

            var result = await _mapper.Map(_request);

            Assert.AreEqual(expectShowConfirmation, result.ShowFlexiJobAgencyDeliveryModelConfirmation);
        }
    }
}
