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

        _outerApiService.Setup(x => x.GetLearnerDetailsForProvider(_request.ProviderId, _request.AccountLegalEntityId, _request.CohortId, _request.SearchTerm, _request.SortField, _request.ReverseSort, _request.Page))
            .ReturnsAsync(_apiResponse);

        _mapper = new SelectLearnerRecordViewModelMapper(_outerApiService.Object);
    }

    [Test]
    public async Task MapToFilterModelCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.FilterModel.ProviderId.Should().Be(_request.ProviderId);
        result.FilterModel.EmployerAccountLegalEntityPublicHashedId.Should().Be(_request.EmployerAccountLegalEntityPublicHashedId);
        result.FilterModel.CohortReference.Should().Be(_request.CohortReference);
        result.FilterModel.TotalNumberOfLearnersFound.Should().Be(_apiResponse.Total);
        result.FilterModel.PageNumber.Should().Be(_apiResponse.Page);
        result.FilterModel.SortField.Should().Be(_request.SortField);
        result.FilterModel.ReverseSort.Should().Be(_request.ReverseSort);
        result.FilterModel.SearchTerm.Should().Be(_request.SearchTerm);
        result.FilterModel.CacheKey.Should().Be(_request.CacheKey);
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
    }

    [Test]
    public async Task MapLearnersCorrectly()
    {
        var result = await _mapper.Map(_request);
        result.Learners.Should().BeEquivalentTo(_apiResponse.Learners.Select(x=>(LearnerSummary)x).ToList());
    }
}