﻿using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class PriceViewModelMapperTests
{
    private PriceViewModelMapper _mapper;
    private PriceRequest _source;
    private Func<Task<PriceViewModel>> _act;
    private Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
    private Mock<ICacheStorageService> _cacheStorage;
    private GetApprenticeshipResponse _getApprenticeshipApiResponse;
    private ChangeEmployerCacheItem _cacheItem;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Build<PriceRequest>().Create();

        _getApprenticeshipApiResponse = new GetApprenticeshipResponse { EmployerName = "TestName" };

        _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
        _commitmentsApiClientMock
            .Setup(x => x.GetApprenticeship(_source.ApprenticeshipId, default(CancellationToken)))
            .ReturnsAsync(_getApprenticeshipApiResponse);

        _cacheStorage = new Mock<ICacheStorageService>();
        _cacheItem = fixture
            .Build<ChangeEmployerCacheItem>()
            .With(x => x.StartDate, "092022")
            .With(x => x.EndDate, "092023")
            .With(x => x.EmploymentEndDate, string.Empty)
            .Create();
            
        _cacheStorage.Setup(x =>
                x.RetrieveFromCache<ChangeEmployerCacheItem>(It.Is<Guid>(key => key == _source.CacheKey)))
            .ReturnsAsync(_cacheItem);

        _mapper = new PriceViewModelMapper(_commitmentsApiClientMock.Object, _cacheStorage.Object);

        _act = async () => await _mapper.Map(TestHelper.Clone(_source));
    }

    [Test]
    public async Task ThenProviderIdMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ApprenticeshipHashedId.Should().Be(_source.ApprenticeshipHashedId);
    }

    [Test]
    public async Task ThenLegalEntityNameIsMappedCorrectly()
    {
        var result = await _act();
        result.LegalEntityName.Should().Be(_getApprenticeshipApiResponse.EmployerName);
    }

    [Test]
    public async Task ThenEditModeIsMappedCorrectly()
    {
        var result = await _act();
        result.InEditMode.Should().Be(_source.IsEdit);
    }

    [Test]
    public async Task ThenApprenticeshipStatusIsMappedCorrectly()
    {
        var result = await _act();
        result.ApprenticeshipStatus.Should().Be(_getApprenticeshipApiResponse.Status);
    }
}