using SFA.DAS.ProviderCommitments.Web.Mappers;
using WebApp = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestToApiCreateCohortRequestMapperTests
{
    [TestFixture]
    public class WhenIMapCreateCohortRequestToApiCreateCohortRequest
    {
        private CreateCohortRequestToApiCreateCohortRequestMapper _mapper;
        private WebApp.CreateCohortRequest _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _mapper = new CreateCohortRequestToApiCreateCohortRequestMapper();

            _source = fixture.Build<WebApp.CreateCohortRequest>()
                .Create();
        }

        [Test]
        public async Task ThenAccountIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.AccountId, result.AccountId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.AccountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.LastName, result.LastName);
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.Email, result.Email);
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.DateOfBirth, result.DateOfBirth);
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.UniqueLearnerNumber, result.Uln);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.Cost, result.Cost);
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.TrainingPrice, result.TrainingPrice);
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.EndPointAssessmentPrice, result.EndPointAssessmentPrice);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.StartDate, result.StartDate);
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ActualStartDate, result.ActualStartDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.EndDate, result.EndDate);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.OriginatorReference, result.OriginatorReference);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
        }
    }
}