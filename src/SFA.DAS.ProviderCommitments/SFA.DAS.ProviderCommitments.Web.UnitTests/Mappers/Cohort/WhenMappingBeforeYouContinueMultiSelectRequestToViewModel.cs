using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using GetConfirmEmployerRequest = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.GetConfirmEmployerRequest;
using GetConfirmEmployerResponse = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.GetConfirmEmployerResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapBeforeYouContinueMultiSelectRequestToBeforeYouContinueMultiSelectViewModel
{
    private BeforeYouContinueMultiSelectRequestViewModelMapper _mapper;
    private Mock<IApprovalsOuterApiClient> _approvalsOuterApiClient;
    private Mock<IOuterApiClient> _outerApiClient;
    private BeforeYouContinueMultiSelectRequest _request;
    private GetHasRelationshipWithPermissionResponse _apiResponse;
    private GetConfirmEmployerResponse _getConfirmEmployerResponse;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _request = fixture.Create<BeforeYouContinueMultiSelectRequest>();
        _apiResponse = fixture.Build<GetHasRelationshipWithPermissionResponse>()
            .With(x => x.HasPermission, true)
            .Create();

        _approvalsOuterApiClient = new Mock<IApprovalsOuterApiClient>();
        _approvalsOuterApiClient
            .Setup(x => x.GetHasRelationshipWithPermission(It.IsAny<long>()))
            .ReturnsAsync(_apiResponse);


        _getConfirmEmployerResponse = fixture.Build<GetConfirmEmployerResponse>()
            .With(x => x.HasNoDeclaredStandards, false)
            .Create();
        _outerApiClient = new Mock<IOuterApiClient>();
        _outerApiClient
            .Setup(x => x.Get<GetConfirmEmployerResponse>(It.IsAny<GetConfirmEmployerRequest>()))
            .ReturnsAsync(_getConfirmEmployerResponse);

        _mapper = new BeforeYouContinueMultiSelectRequestViewModelMapper(_approvalsOuterApiClient.Object, _outerApiClient.Object);
    }

    [Test]
    public async Task ThenApiClientGetConfirmEmployerRequestIsCalled()
    {
        await _mapper.Map(_request);

        _outerApiClient.Verify(x => x.Get<GetConfirmEmployerResponse>(It.IsAny<GetConfirmEmployerRequest>()), Times.Once);
    }

    [Test]
    public async Task ThenApiClientGetHasRelationshipWithPermissionIsCalled()
    {
        await _mapper.Map(_request);

        _approvalsOuterApiClient.Verify(x => x.GetHasRelationshipWithPermission(_request.ProviderId), Times.Once);
    }

    [Test]
    public async Task ThenViewModelIsMappedCorrectly()
    {
        var result = await _mapper.Map(_request);

        result.ProviderId.Should().Be(_request.ProviderId);
        result.HasCreateCohortPermission.Should().Be(_apiResponse.HasPermission);
    }
}