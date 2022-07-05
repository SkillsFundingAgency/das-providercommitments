using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class StartDateViewModelMapperTests
    {
        private StartDateViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new StartDateViewModelMapperFixture();
        }

        [Test]
        public async Task ThenCallsApiClient()
        {
            await _fixture.Act();

            _fixture.Verify_ICommitmentsApiClient_WasCalled(Times.Once());
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Response.StopDate, result.StopDate);
        }

        [Test]
        public async Task ThenCacheKeyIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Request.CacheKey, result.CacheKey);
        }

        [Test]
        public async Task ThenLegalEntityNameIsMapped()
        {
            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.Response.EmployerName, result.LegalEntityName);
        }
    }

    public class StartDateViewModelMapperFixture
    {
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
        private readonly Mock<ICacheStorageService> _cacheStorage;
        private readonly StartDateViewModelMapper _sut;
        private readonly ChangeEmployerCacheItem _cacheItem;

        public StartDateRequest Request { get; }
        public GetApprenticeshipResponse Response { get; }

        public StartDateViewModelMapperFixture()
        {
            var fixture = new Fixture();

            Request = new StartDateRequest
            {
                ApprenticeshipHashedId = "SF45G54",
                ApprenticeshipId = 234,
                ProviderId = 645621,
            };
            Response = new GetApprenticeshipResponse
            {
                StopDate = DateTime.UtcNow.AddDays(-5),
                EmployerName = "TestName"
            };
            _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClientMock
                .Setup(x => x.GetApprenticeship(Request.ApprenticeshipId, CancellationToken.None))
                .ReturnsAsync(Response);

            _cacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "042022")
                .Create();
            _cacheStorage = new Mock<ICacheStorageService>();
            _cacheStorage.Setup(x =>
                    x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(k => k == Request.CacheKey)))
                .ReturnsAsync(_cacheItem);

            _sut = new StartDateViewModelMapper(_commitmentsApiClientMock.Object, _cacheStorage.Object);
        }

        public Task<StartDateViewModel> Act() => _sut.Map(Request);

        public void Verify_ICommitmentsApiClient_WasCalled(Times times)
        {
            _commitmentsApiClientMock
                .Verify(x => x.GetApprenticeship(Request.ApprenticeshipId, CancellationToken.None), times);
        }
    }
}