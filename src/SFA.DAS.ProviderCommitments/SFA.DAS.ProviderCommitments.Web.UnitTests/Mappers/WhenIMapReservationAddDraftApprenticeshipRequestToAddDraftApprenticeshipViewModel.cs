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
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.CohortReference, Is.EqualTo(_source.CohortReference));
        }

        [Test]
        public async Task ThenCohortIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.CohortId, Is.EqualTo(_source.CohortId));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
        }

        [Test]
        public async Task ThenStartMonthYearIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.StartDate.MonthYear, Is.EqualTo(_source.StartMonthYear));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ReservationId, Is.EqualTo(_source.ReservationId));
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.AccountLegalEntityId, Is.EqualTo(_apiResponse.AccountLegalEntityId));
        }

        [Test]
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.HasMultipleDeliveryModelOptions, Is.EqualTo(_apiResponse.HasMultipleDeliveryModelOptions));
        }

        [Test]
        public async Task ThenPublicHashedAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EmployerAccountLegalEntityPublicHashedId, Is.EqualTo("EmployerAccountLegalEntityPublicHashedId"));
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_source.IsOnFlexiPaymentsPilot));
        }
    }
}