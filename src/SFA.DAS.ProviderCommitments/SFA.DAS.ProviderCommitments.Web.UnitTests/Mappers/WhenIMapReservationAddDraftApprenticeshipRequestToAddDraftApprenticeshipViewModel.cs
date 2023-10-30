using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    [TestFixture]
    public class WhenIMapReservationAddDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
    {
        private AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper _mapper;
        private ReservationsAddDraftApprenticeshipRequest _source;
        private Mock<IOuterApiClient> _commitmentsApiClient;
        private Mock<IEncodingService> _encodingService;
        private GetAddDraftApprenticeshipDetailsResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _source = fixture.Build<ReservationsAddDraftApprenticeshipRequest>()
                .With(x => x.StartMonthYear, "042020")
                .With(x => x.CohortId, 1)
                .Create();

            _apiResponse = fixture.Create<GetAddDraftApprenticeshipDetailsResponse>();
            _commitmentsApiClient = new Mock<IOuterApiClient>();
            _commitmentsApiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDetailsResponse>(It.IsAny<GetAddDraftApprenticeshipDetailsRequest>()))
                .ReturnsAsync(_apiResponse);

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Encode(_apiResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId))
                .Returns("EmployerAccountLegalEntityPublicHashedId");

            _mapper = new AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper(_encodingService.Object, _commitmentsApiClient.Object);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task ThenCohortIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.CohortId, result.CohortId);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenStartMonthYearIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.StartMonthYear, result.StartDate.MonthYear);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.AccountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.HasMultipleDeliveryModelOptions, result.HasMultipleDeliveryModelOptions);
        }

        [Test]
        public async Task ThenPublicHashedAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual("EmployerAccountLegalEntityPublicHashedId", result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.IsOnFlexiPaymentsPilot, result.IsOnFlexiPaymentPilot);
        }
    }
}