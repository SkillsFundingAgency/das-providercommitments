using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModel
    {
        private SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper _mapper;
        private EditApprenticeshipRequestViewModel _request;
        private Mock<IOuterApiClient> _outerApiClient;
        private GetEditApprenticeshipDeliveryModelResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartDate).Without(x => x.StartMonth).Without(x => x.StartYear)
                .Without(x => x.EndDate).Without(x => x.EndMonth).Without(x => x.EndYear).Create();

            _outerApiClient = new Mock<IOuterApiClient>();
            _apiResponse = fixture.Create<GetEditApprenticeshipDeliveryModelResponse>();
            _outerApiClient.Setup(x =>
                    x.Get<GetEditApprenticeshipDeliveryModelResponse>(
                        It.Is<GetEditApprenticeshipDeliveryModelRequest>(r =>
                            r.ApprenticeshipId == _request.ApprenticeshipId)))
                .ReturnsAsync(() => _apiResponse);

            _mapper = new SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper(_outerApiClient.Object);
        }

        [Test]
        public async Task Then_DeliveryModels_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_apiResponse.DeliveryModels, result.DeliveryModels);
        }

        [Test]
        public async Task Then_DeliveryModel_Is_Mapped_Correctly()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual((SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel) _request.DeliveryModel, result.DeliveryModel);
        }
    }
}
