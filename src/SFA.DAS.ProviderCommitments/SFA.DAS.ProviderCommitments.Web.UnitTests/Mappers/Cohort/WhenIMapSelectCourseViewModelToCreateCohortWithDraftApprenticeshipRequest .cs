using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using System;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapSelectCourseViewModelToCreateCohortWithDraftApprenticeshipRequest
{
    private CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper _mapper;
    private Web.Models.Cohort.SelectCourseViewModel _source;
    private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;
    private Mock<ICacheStorageService> _cacheService;
    private CreateCohortCacheItem _cacheItem;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Create<Web.Models.Cohort.SelectCourseViewModel>();

        _cacheItem = fixture.Create<CreateCohortCacheItem>();
        _cacheService = new Mock<ICacheStorageService>();
        _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.IsAny<Guid>()))
            .ReturnsAsync(_cacheItem);

        _mapper = new CreateCohortWithDraftApprenticeshipRequestFromSelectCourseViewModelMapper(_cacheService.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMapped()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(_source.AccountLegalEntityId);
    }

    [Test]
    public async Task ThenDeliveryModelIsMappedCorrectly()
    {
        var result = await _act();
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartMonthYear.Should().Be(_source.StartMonthYear);
    }
}