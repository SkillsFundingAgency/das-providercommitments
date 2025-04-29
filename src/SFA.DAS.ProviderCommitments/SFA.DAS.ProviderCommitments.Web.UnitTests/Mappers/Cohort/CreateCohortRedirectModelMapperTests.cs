using System;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class CreateCohortRedirectModelMapperTests
{
    private CreateCohortRedirectModelMapper _mapper;
    private Mock<IAuthorizationService> _authorizationService;
    private Mock<ICacheStorageService> _cacheStorage;
    private CreateCohortWithDraftApprenticeshipRequest _request;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _authorizationService = new Mock<IAuthorizationService>();
        _cacheStorage = new Mock<ICacheStorageService>();

        _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot))
            .ReturnsAsync(true);

        _mapper = new CreateCohortRedirectModelMapper(_authorizationService.Object, _cacheStorage.Object);

        _request = fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
    }

    [TestCase(true, CreateCohortRedirectModel.RedirectTarget.ChooseFlexiPaymentPilotStatus)]
    [TestCase(false, CreateCohortRedirectModel.RedirectTarget.SelectCourse)]
    public async Task Redirect_Target_Is_Mapped_Correctly(bool isFlexiPaymentsEnabled, CreateCohortRedirectModel.RedirectTarget expectTarget)
    {
        _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot))
            .ReturnsAsync(isFlexiPaymentsEnabled);
        _request.UseIlrData = false;

        var result = await _mapper.Map(_request);
        result.RedirectTo.Should().Be(expectTarget);
    }

    [TestCase(true, CreateCohortRedirectModel.RedirectTarget.SelectLearner)]
    [TestCase(false, CreateCohortRedirectModel.RedirectTarget.SelectCourse)]
    public async Task Redirect_Target_Is_Mapped_Correctly_When_UseIlrData(bool useIlrData, CreateCohortRedirectModel.RedirectTarget expectTarget)
    {
        _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot))
            .ReturnsAsync(false);
        _request.UseIlrData = useIlrData;
        var result = await _mapper.Map(_request);
        result.RedirectTo.Should().Be(expectTarget);
    }

    [Test]
    public async Task UseIlrData_Is_Added_To_Cache()
    {
        _request.UseIlrData = true;
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(m => m.UseIlrData == _request.UseIlrData),
                1),
            Times.Once);
    }

    [Test]
    public async Task ReservationId_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(m => m.ReservationId == _request.ReservationId),
                1),
            Times.Once);
    }

    [Test]
    public async Task StartMonthYear_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(m => m.StartMonthYear == _request.StartMonthYear),
                1),
            Times.Once);
    }

    [Test]
    public async Task AccountLegalEntityId_Is_Added_To_Cache()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
                It.Is<CreateCohortCacheItem>(m => m.AccountLegalEntityId == _request.AccountLegalEntityId),
                1),
            Times.Once);
    }

    [Test]
    public async Task CacheKey_Is_Returned()
    {
        var result = await _mapper.Map(_request);
        _cacheStorage.Verify(x => x.SaveToCache(It.Is<Guid>(k => k == result.CacheKey),
                It.IsAny<CreateCohortCacheItem>(), It.IsAny<int>()),
            Times.Once);
    }
}