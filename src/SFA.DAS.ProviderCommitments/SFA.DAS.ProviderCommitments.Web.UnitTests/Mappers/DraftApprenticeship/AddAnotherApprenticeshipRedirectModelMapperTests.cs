using System;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship;

[TestFixture]
public class AddAnotherApprenticeshipRedirectModelMapperTests
{
    private AddAnotherApprenticeshipRedirectModelMapper _mapper;
    private Mock<ICacheStorageService> _cacheStorage;
    private BaseReservationsAddDraftApprenticeshipRequest _request;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _cacheStorage = new Mock<ICacheStorageService>();

        _mapper = new AddAnotherApprenticeshipRedirectModelMapper(_cacheStorage.Object, Mock.Of<ILogger<AddAnotherApprenticeshipRedirectModelMapper>>());

        _request = fixture.Create<BaseReservationsAddDraftApprenticeshipRequest>();
    }

    [TestCase(true, AddAnotherApprenticeshipRedirectModel.RedirectTarget.SelectLearner)]
    [TestCase(false, AddAnotherApprenticeshipRedirectModel.RedirectTarget.SelectCourse)]
    public async Task Redirect_Target_Is_Mapped_Correctly(bool useLearnerData, AddAnotherApprenticeshipRedirectModel.RedirectTarget expectTarget)
    {
        _request.UseLearnerData = useLearnerData;

        var result = await _mapper.Map(_request);
        result.RedirectTo.Should().Be(expectTarget);
    }

    [Test]
    public async Task UseLearnerData_Is_Added_To_Cache()
    {
        _request.UseLearnerData = true;
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<AddAnotherApprenticeshipCacheItem>(m => m.UseLearnerData == _request.UseLearnerData),
                1),
            Times.Once);
    }

    [Test]
    public async Task ReservationId_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<AddAnotherApprenticeshipCacheItem>(m => m.ReservationId == _request.ReservationId),
                1),
            Times.Once);
    }

    [Test]
    public async Task StartMonthYear_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<AddAnotherApprenticeshipCacheItem>(m => m.StartMonthYear == _request.StartMonthYear),
                1),
            Times.Once);
    }

    [Test]
    public async Task AccountCohortReference_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<AddAnotherApprenticeshipCacheItem>(m => m.CohortReference == _request.CohortReference),
                1),
            Times.Once);
    }

    [Test]
    public async Task CacheKey_Is_Returned()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(k => k == result.CacheKey),
                It.IsAny<AddAnotherApprenticeshipCacheItem>(), It.IsAny<int>()),
            Times.Once);
    }
}