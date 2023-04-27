using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using System;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class AddDraftApprenticeshipRedirectModelMapperTests
    {
        private AddDraftApprenticeshipRedirectModelMapper _mapper;
        private AddDraftApprenticeshipViewModel _source;
        private Func<Task<AddDraftApprenticeshipRedirectModel>> _act;
        private Mock<ICacheStorageService> _cacheStorageService;
        private CreateCohortCacheItem _cacheItem;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<AddDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x=>x.StartDate)
                .Without(x => x.EndMonth).Without(x => x.EndYear)
                .Create();
            _source.StartDate = new MonthYearModel("092022");

            _cacheItem = fixture.Create<CreateCohortCacheItem>();
            _cacheStorageService = new Mock<ICacheStorageService>();
            _cacheStorageService
                .Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.Is<Guid>(key => key == _source.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new AddDraftApprenticeshipRedirectModelMapper(_cacheStorageService.Object);

            _act = async () => await _mapper.Map(_source);
        }

        [Test]
        public async Task Then_CacheKey_Is_Mapped_Correctly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CacheKey, result.CacheKey);
        }

        [Test]
        public async Task Then_ProviderId_Is_Mapped_Correctly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task Then_FirstName_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.FirstName == _source.FirstName),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_LastName_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.LastName == _source.LastName),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_Uln_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.Uln == _source.Uln),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_Email_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.Email == _source.Email),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_DateOfBirth_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.DateOfBirth == _source.DateOfBirth.Date),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_StartDate_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.StartDate == _source.StartDate.Date),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_EndDate_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.EndDate == _source.EndDate.Date),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_ActualStartDate_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.ActualStartDate == _source.ActualStartDate.Date),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_EmploymentEndDate_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.EmploymentEndDate == _source.EmploymentEndDate.Date),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_EmploymentPrice_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.EmploymentPrice == _source.EmploymentPrice),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_Cost_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.Cost == _source.Cost),
                It.IsAny<int>()));
        }

        [Test]
        public async Task Then_Reference_Is_Saved_To_Cache()
        {
            var result = await _act();
            _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(y => y.Reference == _source.Reference),
                It.IsAny<int>()));
        }
    }
}
