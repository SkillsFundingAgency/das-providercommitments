using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using System;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapAddDraftApprenticeshipViewModelToCreateCohortWithDraftApprenticeshipRequest
    {
        private CreateCohortWithDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModel _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;
        private Mock<ICacheStorageService> _cacheStorageService;
        private CreateCohortCacheModel _cacheModel;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<AddDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x=>x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            _source.StartDate = new MonthYearModel("092022");

            _cacheModel = fixture.Create<CreateCohortCacheModel>();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService
                .Setup(x => x.RetrieveFromCache<CreateCohortCacheModel>(It.Is<Guid>(key => key == _source.CacheKey)))
                .ReturnsAsync(_cacheModel);

            _mapper = new CreateCohortWithDraftApprenticeshipRequestFromAddDraftApprenticeshipViewModel(_cacheStorageService.Object);

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenAccountLegalEntityIdIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_source.AccountLegalEntityId, result.AccountLegalEntityId);
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.DeliveryModel, result.DeliveryModel);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.StartDate.MonthYear, result.StartMonthYear);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
        }

        [Test]
        public async Task Then_FirstName_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheModel>(y => y.FirstName == _source.FirstName),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_LastName_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheModel>(y => y.LastName == _source.LastName),
                It.IsAny<int>()));
        }
    }
}
