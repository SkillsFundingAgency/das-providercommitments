using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class CreateChangeOfPartyRequestMapperTests
    {
        private CreateChangeOfPartyRequestMapper _mapper;
        private ConfirmViewModel _source;
        private CreateChangeOfPartyRequestRequest _result;
        private Mock<ICacheStorageService> _cacheService;
        private ChangeEmployerCacheItem _cacheItem;

        [SetUp]
        public async Task Arrange()
        {
            var fixture = new Fixture();

            _source = new ConfirmViewModel
            {
                CacheKey = Guid.NewGuid()
            };

            _cacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "032021")
                .With(x => x.EndDate, "092021")
                .With(x => x.EmploymentEndDate, "")
                .Create();
            _cacheService = new Mock<ICacheStorageService>();
            _cacheService.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(k => k == _source.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _mapper = new CreateChangeOfPartyRequestMapper(_cacheService.Object);
            _result = await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public void ChangeOfPartyRequestTypeIsMappedCorrectly()
        {
            Assert.AreEqual(ChangeOfPartyRequestType.ChangeEmployer, _result.ChangeOfPartyRequestType);
        }

        [Test]
        public void NewPartyIdIsMappedCorrectly()
        {
            Assert.AreEqual(_cacheItem.AccountLegalEntityId, _result.NewPartyId);
        }

        [Test]
        public void NewPriceIsMappedCorrectly()
        {
            Assert.AreEqual(_cacheItem.Price, _result.NewPrice);
        }

        [Test]
        public void NewStartDateIsMappedCorrectly()
        {
            Assert.AreEqual(new MonthYearModel(_cacheItem.StartDate).Date, _result.NewStartDate);
        }

        [Test]
        public void NewEndDateIsMappedCorrectly()
        {
            Assert.AreEqual(new MonthYearModel(_cacheItem.EndDate).Date, _result.NewEndDate);
        }
    }
}
