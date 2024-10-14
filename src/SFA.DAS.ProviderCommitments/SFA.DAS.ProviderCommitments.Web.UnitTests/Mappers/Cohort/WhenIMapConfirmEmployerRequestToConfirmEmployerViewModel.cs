using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapConfirmEmployerRequestToConfirmEmployerViewModel
{
    private ConfirmEmployerRequestToViewModelMapper _mapper;
    private ConfirmEmployerRequest _source;
    private Func<Task<ConfirmEmployerViewModel>> _act;
    private CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse _accountLegalEntityResponse;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _accountLegalEntityResponse = fixture.Create<CommitmentsV2.Api.Types.Responses.AccountLegalEntityResponse>();
        _source = fixture.Create<ConfirmEmployerRequest>();
        var icommitmentApiClient = new Mock<ICommitmentsApiClient>();
        icommitmentApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_accountLegalEntityResponse);

        _mapper = new ConfirmEmployerRequestToViewModelMapper(icommitmentApiClient.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityPublicHashedId.Should().Be(_source.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public async Task ThenEmployerAccountNameIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountName.Should().Be(_accountLegalEntityResponse.AccountName);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityNameIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerAccountLegalEntityName.Should().Be(_accountLegalEntityResponse.LegalEntityName);
    }

    [Test]
    public async Task ThenProviderIdMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsNotMapped()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(0);
    }
}