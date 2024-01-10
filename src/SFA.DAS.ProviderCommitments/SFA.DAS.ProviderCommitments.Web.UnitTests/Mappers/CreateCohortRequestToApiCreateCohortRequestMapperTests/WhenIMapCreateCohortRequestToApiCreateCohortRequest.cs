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
            Assert.That(result.AccountId, Is.EqualTo(_source.AccountId));
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.AccountLegalEntityId, Is.EqualTo(_source.AccountLegalEntityId));
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.FirstName, Is.EqualTo(_source.FirstName));
        }

        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.LastName, Is.EqualTo(_source.LastName));
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Email, Is.EqualTo(_source.Email));
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.DateOfBirth, Is.EqualTo(_source.DateOfBirth));
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Uln, Is.EqualTo(_source.UniqueLearnerNumber));
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.CourseCode, Is.EqualTo(_source.CourseCode));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Cost, Is.EqualTo(_source.Cost));
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.TrainingPrice, Is.EqualTo(_source.TrainingPrice));
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(_source.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.StartDate, Is.EqualTo(_source.StartDate));
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ActualStartDate, Is.EqualTo(_source.ActualStartDate));
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EndDate, Is.EqualTo(_source.EndDate));
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.OriginatorReference, Is.EqualTo(_source.OriginatorReference));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ReservationId, Is.EqualTo(_source.ReservationId));
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.DeliveryModel, Is.EqualTo(_source.DeliveryModel));
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_source.IsOnFlexiPaymentPilot));
        }
    }
}