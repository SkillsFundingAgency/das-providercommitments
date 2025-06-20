using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class WhenIMapSelectedLearnerRequestToCreateCohortWithDraftApprenticeshipRequest
{
    private CreateCohortWithDraftApprenticeshipRequestFromLearnerSelectedRequestMapper _mapper;
    private LearnerSelectedRequest _source;
    private long _providerId;
    private long _learnerId;
    private GetLearnerSelectedResponse _response;
    private Func<Task<CreateCohortWithDraftApprenticeshipRequest>> _act;
    private Mock<ICacheStorageService> _cacheService;
    private Mock<IOuterApiService> _outerApiService;
    private CreateCohortCacheItem _cacheItem;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _providerId = fixture.Create<long>();
        _learnerId = fixture.Create<long>();
        _response = fixture.Create<GetLearnerSelectedResponse>();
        _source = fixture.Build<LearnerSelectedRequest>().With(p=>p.ProviderId, _providerId).With(p=>p.LearnerDataId, _learnerId).Create();

        _cacheItem = fixture.Create<CreateCohortCacheItem>();
        _cacheService = new Mock<ICacheStorageService>();
        _cacheService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(_source.CacheKey)).ReturnsAsync(_cacheItem);

        _outerApiService = new Mock<IOuterApiService>();
        _outerApiService.Setup(x => x.GetLearnerSelected(_providerId, _learnerId)).ReturnsAsync(_response);

        _mapper = new CreateCohortWithDraftApprenticeshipRequestFromLearnerSelectedRequestMapper(_cacheService.Object, _outerApiService.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public async Task ThenCacheKeyIsMappedCorrectly()
    {
        var result = await _act();
        result.CacheKey.Should().Be(_source.CacheKey);
    }

    [Test]
    public async Task ThenReservationKeyIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_cacheItem.ReservationId);
    }

    [Test]
    public async Task ThenAleIdIsMappedCorrectly()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(_source.AccountLegalEntityId);
    }

    [Test]
    public async Task ThenVerifyCacheUpdatedWithPersonalDetailsCorrectly()
    {

        var result = await _act();
        _cacheService.Verify(x=>x.SaveToCache(_cacheItem.CacheKey, It.Is<CreateCohortCacheItem>(p=>ValidateDetails(p)), 1));;
    }

    [TestCase(true, DeliveryModel.FlexiJobAgency)]
    [TestCase(false, DeliveryModel.Regular)]
    public async Task ThenVerifyCacheUpdatedWithTheseDetailsCorrectly(bool isFlexiJob, DeliveryModel? expected)
    {
        _response.IsFlexiJob = isFlexiJob;
        var result = await _act();
        _cacheService.Verify(x => x.SaveToCache(_cacheItem.CacheKey, It.Is<CreateCohortCacheItem>(p => p.DeliveryModel == expected), 1)); ;
    }

    private bool ValidateDetails(CreateCohortCacheItem i)
    {
        return i.FirstName == _response.FirstName &&
               i.LastName == _response.LastName &&
               i.Email == _response.Email &&
               i.DateOfBirth == _response.Dob &&
               i.StartDate == _response.StartDate &&
               i.EndDate == _response.PlannedEndDate &&
               i.Uln == _response.Uln.ToString() &&
               i.LearnerDataId == _source.LearnerDataId &&
               i.EndPointAssessmentPrice == _response.EpaoPrice &&
               i.TrainingPrice == _response.TrainingPrice &&
               i.CourseCode == _response.StandardCode.ToString() &&
               i.Cost == _response.TrainingPrice + _response.EpaoPrice;
    }
}