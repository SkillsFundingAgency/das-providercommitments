using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeship;

internal class DraftApprenticeshipSetReferenceRequestToViewModelMapperTests
{
    private DraftApprenticeshipSetReferenceRequestToViewModelMapper _mapper;
    private Mock<IOuterApiClient> _apiClient;
    private DraftApprenticeshipRequest _request;
    private GetEditDraftApprenticeshipResponse _apiResponse;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _request = _fixture.Create<DraftApprenticeshipRequest>();
        _apiResponse = _fixture.Create<GetEditDraftApprenticeshipResponse>();

        _apiClient = new Mock<IOuterApiClient>();
        _apiClient.Setup(x => x.Get<GetEditDraftApprenticeshipResponse>(It.Is<GetEditDraftApprenticeshipRequest>(r =>
               r.DraftApprenticeshipId == _request.DraftApprenticeshipId
               && r.CohortId == _request.CohortId
               && r.ProviderId == _request.ProviderId)))
           .ReturnsAsync(_apiResponse);

        _mapper = new DraftApprenticeshipSetReferenceRequestToViewModelMapper(_apiClient.Object);
    }

    [Test]
    public async Task ProviderId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.ProviderId.Should().Be(_request.ProviderId);
    }

    [Test]
    public async Task CohortId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.CohortId.Should().Be(_request.CohortId);
    }

    [Test]
    public async Task DraftApprenticeshipHashedId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.DraftApprenticeshipHashedId.Should().Be(_request.DraftApprenticeshipHashedId);
    }

    [Test]
    public async Task Name_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.Name.Should().Be($"{_apiResponse.FirstName} {_apiResponse.LastName}");
    }

    [Test]
    public async Task Email_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.Reference.Should().Be(_apiResponse.ProviderReference);
    }

    [Test]
    public async Task OriginalReference_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.OriginalReference.Should().Be(_apiResponse.ProviderReference);
    }

    [Test]
    public async Task CohortReference_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.CohortReference.Should().Be(_request.CohortReference);
    }

    [Test]
    public async Task DraftApprenticeshipId_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.DraftApprenticeshipId.Should().Be(_request.DraftApprenticeshipId);
    }
}
