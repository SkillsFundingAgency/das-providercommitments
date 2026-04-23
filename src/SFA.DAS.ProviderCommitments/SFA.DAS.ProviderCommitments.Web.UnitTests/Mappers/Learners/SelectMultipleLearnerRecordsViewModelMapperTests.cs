using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectMultipleLearnerRecordsViewModelMapperTests
{
    private SelectMultipleLearnerRecordsViewModelMapper _mapper;
    private Mock<IOuterApiService> _outerApiService;
    private Mock<ICacheStorageService> _cacheStorage;

    private SelectMultipleLearnerRecordsRequest _request;
    private SelectMultipleLearnerRecordsCacheItem _cacheItem;
    private GetLearnerDetailsForProviderResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _request = fixture.Create<SelectMultipleLearnerRecordsRequest>();

        _cacheItem = fixture.Build<SelectMultipleLearnerRecordsCacheItem>()
            .With(x => x.StartMonth, "1")
            .With(x => x.StartYear, "2025")
            .With(x => x.CourseCode, "12")
            .Create();

        _apiResponse = fixture.Create<GetLearnerDetailsForProviderResponse>();
        foreach (var learner in _apiResponse.Learners)
        {
            learner.LearningType = "Apprenticeship";
        }

        _outerApiService = new Mock<IOuterApiService>();
        _cacheStorage = new Mock<ICacheStorageService>();

        _cacheStorage
            .Setup(x => x.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(_request.CacheKey.Value))
            .ReturnsAsync(_cacheItem);

        _outerApiService.Setup(x => x.GetLearnerDetailsForProvider(_cacheItem.ProviderId,
            It.Is<SelectLearnersRequest>(t => t.AccountLegalEntityId == _cacheItem.AccountLegalEntityId &&
            t.CohortId == _cacheItem.CohortId &&
            t.SearchTerm == _cacheItem.SearchTerm &&
            t.SortColumn == _cacheItem.SortField &&
            t.ReverseSort == _cacheItem.ReverseSort &&
            t.Page == _request.Page &&
            t.StartMonth == 1 &&
            t.StartYear == 2025 &&
            t.CourseCode == _cacheItem.CourseCode)))
            .ReturnsAsync(_apiResponse);

        _mapper = new SelectMultipleLearnerRecordsViewModelMapper(_outerApiService.Object, _cacheStorage.Object);
    }

    [Test]
    public async Task MapToFilterModelCorrectly()
    {
        var result = await _mapper.Map(_request);

        result.FilterModel.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.FilterModel.EmployerAccountLegalEntityPublicHashedId.Should().Be(_cacheItem.EmployerAccountLegalEntityPublicHashedId);
        result.FilterModel.CohortReference.Should().Be(_cacheItem.CohortReference);

        result.FilterModel.CacheKey.Should().Be(_cacheItem.CacheKey);

        result.FilterModel.TotalNumberOfLearnersFound.Should().Be(_apiResponse.Total);
        result.FilterModel.PageNumber.Should().Be(_request.Page);

        result.FilterModel.SortField.Should().Be(_cacheItem.SortField);
        result.FilterModel.ReverseSort.Should().Be(_cacheItem.ReverseSort);
        result.FilterModel.SearchTerm.Should().Be(_cacheItem.SearchTerm);

        result.FilterModel.StartMonth.Should().Be(_cacheItem.StartMonth);
        result.FilterModel.StartYear.Should().Be(_cacheItem.StartYear);
        result.FilterModel.CourseCode.Should().Be(_cacheItem.CourseCode);
    }

    [Test]
    public async Task MapToViewModelCorrectly()
    {
        var result = await _mapper.Map(_request);

        result.ProviderId.Should().Be(_cacheItem.ProviderId);
        result.CohortReference.Should().Be(_cacheItem.CohortReference);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_cacheItem.EmployerAccountLegalEntityPublicHashedId);
        result.CacheKey.Should().Be(_request.CacheKey);
        result.EmployerAccountName.Should().Be(_cacheItem.EmployerAccountName);
        result.LastIlrSubmittedOn.Should().Be(_apiResponse.LastSubmissionDate);
        result.FutureMonths.Should().Be(_apiResponse.FutureMonths);
        result.FilterModel.Courses.Where(c => c.Text != "All").Select(x => x.Text).
           Should().BeEquivalentTo(_apiResponse.TrainingCourses.Select(y => y.Name));
    }

    [Test]
    public async Task MapLearnersCorrectly()
    {
        var result = await _mapper.Map(_request);

        var expected = _apiResponse.Learners.Select(x => (LearnerSummary)x).ToList();
        result.Learners.Should().BeEquivalentTo(expected);

    }
}