using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

public class WhenIMapConfirmEmployerViewModelToCreateEmptyCohortRequest
{
    private ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper _mapper;
    private ConfirmEmployerViewModel _source;
    private Func<Task<CreateEmptyCohortRequest>> _act;
    private Mock<ICommitmentsApiClient> _commitmentApiClient;
    private AccountLegalEntityResponse _accountLegalEntityResponse;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _commitmentApiClient = new Mock<ICommitmentsApiClient>();
        _accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();

        _source = fixture.Create<ConfirmEmployerViewModel>();
        _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

        _mapper = new ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper(_commitmentApiClient.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenProviderIdMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(_source.AccountLegalEntityId);
    }

    [Test]
    public async Task ThenAccountIdMappedCorrectly()
    {
        var result = await _act();
        result.AccountId.Should().Be(_accountLegalEntityResponse.AccountId);
    }
}