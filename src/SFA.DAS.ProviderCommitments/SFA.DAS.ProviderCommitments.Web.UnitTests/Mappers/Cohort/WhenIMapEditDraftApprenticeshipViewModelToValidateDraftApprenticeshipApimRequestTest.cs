using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapEditDraftApprenticeshipViewModelToValidateDraftApprenticeshipApimRequestTest
    {
        private EditDraftApprenticeshipViewModelToValidateApimRequestMapper _mapper;
        private EditDraftApprenticeshipViewModel _source;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _mapper = new EditDraftApprenticeshipViewModelToValidateApimRequestMapper();

            _source = fixture.Build<EditDraftApprenticeshipViewModel>()
                
                .With(x => x.BirthDay, 1).With(x => x.BirthMonth, 1).With(x => x.BirthYear, 2000)
                .Without(x => x.StartDate)
                .With(x => x.StartMonth,2).With(x => x.StartYear, 2020)
                .With(x => x.IsOnFlexiPaymentPilot, false)
                .With(x => x.EndMonth, 1)
                .With(x => x.EndYear, 2022)
                .Create();
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
            Assert.That(result.DateOfBirth, Is.EqualTo(_source.DateOfBirth.Date));
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.Uln, Is.EqualTo(_source.Uln));
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
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.StartDate, Is.EqualTo(_source.StartDate.Date));
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.EndDate, Is.EqualTo(_source.EndDate.Date));
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.OriginatorReference, Is.EqualTo(_source.Reference));
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

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.That(result.ActualStartDate, Is.EqualTo(_source.ActualStartDate.Date));
        }
    }
}
