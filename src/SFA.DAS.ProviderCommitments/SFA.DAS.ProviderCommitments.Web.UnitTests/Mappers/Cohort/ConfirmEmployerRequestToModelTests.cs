using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class ConfirmEmployerRequestToModelTests
{
    private ConfirmEmployerRequestToModelMapper _mapper;
    private Mock<IOuterApiClient> _apiClient;
    private ConfirmEmployerViewModel _request;
    private GetConfirmEmployerResponse _apiResponse;
    private readonly Fixture _fixture = new();

    [Test]
    public async Task HasNoDeclaredStandards_Is_Mapped_Correctly()
    {
        var result = await _mapper.Map(_request);
        result.HasNoDeclaredStandards.Should().Be(_apiResponse.HasNoDeclaredStandards);
    }

    [SetUp]
    public void Setup()
    {
        _request = _fixture.Create<ConfirmEmployerViewModel>();
        _apiResponse = _fixture.Create<GetConfirmEmployerResponse>();

        _apiClient = new Mock<IOuterApiClient>();
        _apiClient.Setup(x => x.Get<GetConfirmEmployerResponse>(It.Is<GetConfirmEmployerRequest>(r =>
                r.ProviderId == _request.ProviderId)))
            .ReturnsAsync(_apiResponse);

        _mapper = new ConfirmEmployerRequestToModelMapper(_apiClient.Object);
    }
}