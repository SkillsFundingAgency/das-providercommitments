using System;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;

[TestFixture]
public class WhenIMapReservationAddDraftApprenticeshipRequestToAddDraftApprenticeshipViewModel
{
    private AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper _mapper;
    private ReservationsAddDraftApprenticeshipRequest _source;
    private Mock<IOuterApiClient> _commitmentsApiClient;
    private Mock<IEncodingService> _encodingService;
    private Mock<ICacheStorageService> _cacheService;
    private GetAddDraftApprenticeshipDetailsResponse _apiResponse;
    private AddAnotherApprenticeshipCacheItem _cacheResponse;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _source = fixture.Build<ReservationsAddDraftApprenticeshipRequest>()
            .With(x => x.StartMonthYear, "042020")
            .With(x => x.CohortId, 1)
            .Create();

        _cacheResponse = fixture.Create<AddAnotherApprenticeshipCacheItem>();

        _apiResponse = fixture.Create<GetAddDraftApprenticeshipDetailsResponse>();
        _commitmentsApiClient = new Mock<IOuterApiClient>();
        _commitmentsApiClient.Setup(x => x.Get<GetAddDraftApprenticeshipDetailsResponse>(It.IsAny<GetAddDraftApprenticeshipDetailsRequest>()))
            .ReturnsAsync(_apiResponse);

        _encodingService = new Mock<IEncodingService>();
        _encodingService.Setup(x => x.Encode(_apiResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId))
            .Returns("EmployerAccountLegalEntityPublicHashedId");

        _cacheService = new Mock<ICacheStorageService>();
        _cacheService.Setup(x => x.RetrieveFromCache<AddAnotherApprenticeshipCacheItem>(It.IsAny<Guid>())).ReturnsAsync(_cacheResponse);

        _mapper = new AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper(_encodingService.Object, _commitmentsApiClient.Object, _cacheService.Object);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenCohortReferenceIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.CohortReference.Should().Be(_source.CohortReference);
    }

    [Test]
    public async Task ThenCohortIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.CohortId.Should().Be(_source.CohortId);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        _source.CacheKey = null;
        var result = await _mapper.Map(_source);
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenStartMonthYearIsMappedCorrectly()
    {
        _source.CacheKey = null;
        var result = await _mapper.Map(_source);
        result.StartDate.MonthYear.Should().Be(_source.StartMonthYear);
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        _source.CacheKey = null;
        var result = await _mapper.Map(_source);
        result.ReservationId.Should().Be(_source.ReservationId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
    {
        _source.CacheKey = null;
        var result = await _mapper.Map(_source);
        result.AccountLegalEntityId.Should().Be(_apiResponse.AccountLegalEntityId);
    }

    [Test]
    public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.HasMultipleDeliveryModelOptions.Should().Be(_apiResponse.HasMultipleDeliveryModelOptions);
    }

    [Test]
    public async Task ThenPublicHashedAccountLegalEntityIdIsMappedCorrectly()
    {
        var result = await _mapper.Map(_source);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be("EmployerAccountLegalEntityPublicHashedId");
    }

    [Test]
    public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
    {
        _source.CacheKey = null;
        var result = await _mapper.Map(_source);
        result.IsOnFlexiPaymentPilot.Should().Be(_source.IsOnFlexiPaymentPilot);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task AndHasCacheKeyThenIsOnFlexiPaymentPilotIsAlwaysFalse(bool isFlexiPayment)
    {
        _source.IsOnFlexiPaymentPilot = isFlexiPayment;
        var result = await _mapper.Map(_source);
        result.IsOnFlexiPaymentPilot.Should().BeFalse();
    }

    [Test]
    public async Task AndHasCacheKeyThenReservationIdIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.ReservationId.Should().Be(_cacheResponse.ReservationId);
    }

    [Test]
    public async Task AndHasCacheKeyThenNameIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.FirstName.Should().Be(_cacheResponse.FirstName);
        result.LastName.Should().Be(_cacheResponse.LastName);
    }

    [Test]
    public async Task AndHasCacheKeyThenEmailIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.Email.Should().Be(_cacheResponse.Email);
    }

    [Test]
    public async Task AndHasCacheKeyThenDoBIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.DateOfBirth.Day.Should().Be(_cacheResponse.DateOfBirth.Value.Day);
        result.DateOfBirth.Month.Should().Be(_cacheResponse.DateOfBirth.Value.Month);
        result.DateOfBirth.Year.Should().Be(_cacheResponse.DateOfBirth.Value.Year);
    }

    [Test]
    public async Task AndHasCacheKeyThenStartDateIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.StartDate.Day.Should().Be(1);
        result.StartDate.Month.Should().Be(_cacheResponse.StartDate.Value.Month);
        result.StartDate.Year.Should().Be(_cacheResponse.StartDate.Value.Year);
    }

    [Test]
    public async Task AndHasCacheKeyThenEndDateIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.EndDate.Day.Should().Be(1);
        result.EndDate.Month.Should().Be(_cacheResponse.EndDate.Value.Month);
        result.EndDate.Year.Should().Be(_cacheResponse.EndDate.Value.Year);
    }

    [Test]
    public async Task AndHasCacheKeyThenUlnIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.Uln.Should().Be(_cacheResponse.Uln);
    }

    [Test]
    public async Task AndHasCacheKeyThenCourseCodeIsMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.CourseCode.Should().Be(_cacheResponse.CourseCode);
    }

    [Test]
    public async Task AndHasCacheKeyThenCostsAreMappedFromCache()
    {
        var result = await _mapper.Map(_source);
        result.Cost.Should().Be(_cacheResponse.Cost);
        result.TrainingPrice.Should().Be(_cacheResponse.TrainingPrice);
        result.EndPointAssessmentPrice.Should().Be(_cacheResponse.EndPointAssessmentPrice);
    }

    [TestCase(Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency, DeliveryModel.FlexiJobAgency)]
    [TestCase(Infrastructure.OuterApi.Types.DeliveryModel.Regular, DeliveryModel.Regular)]
    [TestCase(Infrastructure.OuterApi.Types.DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
    public async Task AndHasCacheKeyThenDeliveryModelIsMappedFromCache(Infrastructure.OuterApi.Types.DeliveryModel dm, DeliveryModel expected)
    {
        _cacheResponse.DeliveryModel = dm;
        var result = await _mapper.Map(_source);
        result.DeliveryModel.Should().Be(expected);
    }
}