using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapReservationAddDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
    {
        private AddDraftApprenticeshipViewModelFromSelectedReservationMapper _mapper;
        private ReservationsAddDraftApprenticeshipRequest _source;
        private Mock<IAuthorizationService> _authorizationService;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _source = fixture.Build<ReservationsAddDraftApprenticeshipRequest>().With(x => x.StartMonthYear, "042020").Create();

            _authorizationService = new Mock<IAuthorizationService>();
            
            _mapper = new AddDraftApprenticeshipViewModelFromSelectedReservationMapper(_authorizationService.Object);
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

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenShowEmailIsMappedCorrectly(bool showEmail)
        {
            _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail)).ReturnsAsync(showEmail);
            var result = await _mapper.Map(_source);
            Assert.AreEqual(showEmail, result.ShowEmail);
        }
    }
}