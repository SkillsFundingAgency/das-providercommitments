using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class EndDateViewModelMapperTests
    {
        private EndDateViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new EndDateViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_fixture.Request.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ThenLegalEntityNameIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.LegalEntityName, Is.EqualTo(_fixture.ApprenticeshipResponse.EmployerName));
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.That(result.ProviderId, Is.EqualTo(_fixture.Request.ProviderId));
        }

        [TestCase(null)]
        [TestCase("022020")]
        public async Task ThenStartDateIsMapped(string startDate)
        {
            _fixture.CacheItem.StartDate = startDate;
            var result = await _fixture.Act();

            Assert.That(result.StartDate, Is.EqualTo(startDate));
        }


        [TestCase("")]
        [TestCase("042019")]
        public async Task ThenEndDateIsMapped(string endDate)
        {
            _fixture.CacheItem.EndDate = endDate;
            var result = await _fixture.Act();

            Assert.That(result.EndDate.MonthYear, Is.EqualTo(_fixture.CacheItem.EndDate));
        }

        [Test]
        public async Task ThenEditModeIsMapped()
        {
            _fixture.Request.IsEdit = true;
            var result = await _fixture.Act();

            Assert.IsTrue(result.InEditMode);
        }
    }

    internal class EndDateViewModelMapperFixture
    {
        private readonly EndDateViewModelMapper _sut;
        public readonly ChangeEmployerCacheItem CacheItem;
        public EndDateRequest Request { get; }

        public GetApprenticeshipResponse ApprenticeshipResponse { get; }

        public EndDateViewModelMapperFixture()
        {
            var fixture = new Fixture();

            Request = new EndDateRequest
            {
                ApprenticeshipHashedId = "SF45G54",
                ApprenticeshipId = 234,
                ProviderId = 645621
            };

            ApprenticeshipResponse = new GetApprenticeshipResponse { EmployerName = "TestName" };
            var commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();

            commitmentsApiClientMock
                .Setup(x => x.GetApprenticeship(Request.ApprenticeshipId, default(CancellationToken)))
                .ReturnsAsync(ApprenticeshipResponse);

            CacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "022022")
                .With(x => x.EndDate, "022022")
                .With(x => x.EmploymentEndDate, "022022")
                .Create();
            var cacheStorageService = new Mock<ICacheStorageService>();
            cacheStorageService.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(CacheItem);

            _sut = new EndDateViewModelMapper(commitmentsApiClientMock.Object, cacheStorageService.Object);
        }

        public Task<EndDateViewModel> Act() => _sut.Map(Request);
    }
}