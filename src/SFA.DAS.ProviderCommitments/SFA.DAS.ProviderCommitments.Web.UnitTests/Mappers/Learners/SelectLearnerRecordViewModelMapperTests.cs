using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Learners;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Learners;

[TestFixture]
public class SelectLearnerRecordViewModelMapperTests
{
    private SelectLearnerRecordViewModelMapper _mapper;
    private Mock<IOuterApiService> _outerApiService;
    private SelectLearnerRecordRequest _request;
    private GetLearnerDetailsForProviderResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _request = fixture.Create<SelectLearnerRecordRequest>();
        _apiResponse = fixture.Create<GetLearnerDetailsForProviderResponse>();

        _outerApiService = new Mock<IOuterApiService>();

        _outerApiService.Setup(x => x.GetLearnerDetailsForProvider(_request.ProviderId,
            It.Is<SelectLearnersRequest>(t => t.AccountLegalEntityId == _request.AccountLegalEntityId &&
            t.CohortId == _request.CohortId &&
            t.SearchTerm == _request.SearchTerm &&
            t.SortColumn == _request.SortField &&
            t.ReverseSort == _request.ReverseSort &&
            t.Page == _request.Page &&
            t.StartMonth == _request.StartMonth &&
            t.StartYear == _request.StartYear &&
            t.CourseCode == _request.CourseCode)))
            .ReturnsAsync(_apiResponse);

        _mapper = new SelectLearnerRecordViewModelMapper(_outerApiService.Object);
    }

    [Test]
    public async Task MapToFilterModelCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.FilterModel.ProviderId.Should().Be(_request.ProviderId);
        result.FilterModel.StartYear.Should().Be(_request.StartYear.ToString());
        result.FilterModel.StartMonth.Should().Be(_request.StartMonth.ToString());
        result.FilterModel.EmployerAccountLegalEntityPublicHashedId.Should().Be(_request.EmployerAccountLegalEntityPublicHashedId);
        result.FilterModel.CohortReference.Should().Be(_request.CohortReference);
        result.FilterModel.TotalNumberOfLearnersFound.Should().Be(_apiResponse.Total);
        result.FilterModel.PageNumber.Should().Be(_apiResponse.Page);
        result.FilterModel.SortField.Should().Be(_request.SortField);
        result.FilterModel.ReverseSort.Should().Be(_request.ReverseSort);
        result.FilterModel.SearchTerm.Should().Be(_request.SearchTerm);
        result.FilterModel.CacheKey.Should().Be(_request.CacheKey);
        result.FilterModel.CourseCode.Should().Be(_request.CourseCode);
    }

    [Test]
    public async Task MapToViewModelCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.ProviderId.Should().Be(_request.ProviderId);
        result.CacheKey.Should().Be(_request.CacheKey);
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_request.EmployerAccountLegalEntityPublicHashedId);
        result.CohortReference.Should().Be(_request.CohortReference);
        result.EmployerAccountName.Should().Be(_apiResponse.EmployerName);
        result.LastIlrSubmittedOn.Should().Be(_apiResponse.LastSubmissionDate);
        result.FutureMonths.Should().Be(_apiResponse.FutureMonths);
        result.FilterModel.Courses.Where(c => c.Text != "All").Select(x => x.Text).
            Should().BeEquivalentTo(_apiResponse.TrainingCourses.Select(y => y.Name));
    }

    [Test]
    public async Task MapLearnersCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.Learners.Should().BeEquivalentTo(_apiResponse.Learners.Select(x => (LearnerSummary)x).ToList());
    }
}