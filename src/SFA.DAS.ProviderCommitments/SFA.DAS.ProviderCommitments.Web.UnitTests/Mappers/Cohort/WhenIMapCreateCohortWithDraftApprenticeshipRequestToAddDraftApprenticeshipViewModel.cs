using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenIMapCreateCohortWithDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
    {
        private AddDraftApprenticeshipViewModelMapper _mapper;
        private CreateCohortWithDraftApprenticeshipRequest _source;
        private Mock<IOuterApiClient> _apiClient;
        private GetAddDraftApprenticeshipDetailsResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _source = fixture.Build<CreateCohortWithDraftApprenticeshipRequest>().With(x => x.StartMonthYear, "042020").Create();
            _apiResponse = fixture.Create<GetAddDraftApprenticeshipDetailsResponse>();

            _apiClient = new Mock<IOuterApiClient>();
            _apiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDetailsResponse>(It.IsAny<GetAddDraftApprenticeshipDetailsRequest>()))
                .ReturnsAsync(_apiResponse);

            _mapper = new AddDraftApprenticeshipViewModelMapper(_apiClient.Object);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.AccountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.LegalEntityName, result.Employer);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCourseNameIsNullWhenNoCourseCode()
        {
            _source.CourseCode = null;
            var result = await _mapper.Map(_source);
            Assert.IsNull(result.CourseName);
            Assert.IsNull(result.CourseCode);
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
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_apiResponse.HasMultipleDeliveryModelOptions, result.HasMultipleDeliveryModelOptions);
        }
    }
}