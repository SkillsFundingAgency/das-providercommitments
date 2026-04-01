using System.Linq;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class SelectEmployerViewModelMapperTests
{
    [Test]
    public async Task ThenCallsProviderRelationshipsApiClient()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        await fixture.Act();

        fixture.Verify_GetSelectNewEmployer_Once();
    }

    [Test]
    public async Task ThenCorrectlyMapsApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Act();

        fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
    }

    [Test]
    public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture().WithNoMatchingEmployers();

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
    }
}

public class SelectEmployerViewModelMapperFixture
{
    private readonly SelectEmployerViewModelMapper _sut;
    private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
    private readonly SelectEmployerRequest _request;
    private readonly long _accountLegalEntityId;
    private readonly long _apprenticeshipId;
    private readonly GetSelectEmployerResponse _apiResponse;

    public GetApprenticeshipResponse GetApprenticeshipApiResponse { get; }

    public SelectEmployerViewModelMapperFixture()
    {
        const long providerId = 123;
        _accountLegalEntityId = 457;
        _apprenticeshipId = 1;
        _request = new SelectEmployerRequest { ProviderId = providerId, ApprenticeshipId = _apprenticeshipId };
        _apiResponse = new GetSelectEmployerResponse
        {
            AccountProviderLegalEntities =
               [
                   new()
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "DSFF23",
                        AccountLegalEntityName = "TestAccountLegalEntityName",
                        AccountPublicHashedId = "DFKFK66",
                        AccountName = "TestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234,
                        AccountHashedId = "HASH1",
                        ApprenticeshipEmployerType = "Levy"
                    }
               ],
            Employers = ["TestAccountLegalEntityName", "TestAccountName"],
            TotalCount = 1,
            EmployerName = "TestAccountLegalEntityName"
        };

        GetApprenticeshipApiResponse = new GetApprenticeshipResponse
        {
            AccountLegalEntityId = _accountLegalEntityId,
            EmployerName = "TestName"
        };

        _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
        _approvalsOuterApiClientMock
            .Setup(x => x.GetSelectNewEmployer(It.IsAny<GetSelectNewEmployerRequest>()))
            .ReturnsAsync(_apiResponse);

        _sut = new SelectEmployerViewModelMapper(_approvalsOuterApiClientMock.Object);
    }

    public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);

    public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
    {
        _approvalsOuterApiClientMock
            .Setup(x => x.GetSelectNewEmployer(It.IsAny<GetSelectNewEmployerRequest>()))
            .ReturnsAsync(new GetSelectEmployerResponse() { AccountProviderLegalEntities = null });

        return this;
    }

    public void Verify_GetSelectNewEmployer_Once()
    {
        _approvalsOuterApiClientMock.Verify(x => x.GetSelectNewEmployer(It.IsAny<GetSelectNewEmployerRequest>()), Times.Once);
    }

    public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
    {
        var filteredLegalEntities = _apiResponse.AccountProviderLegalEntities;
        using (new AssertionScope())
        {
            result.LegalEntityName.Should().Be(_apiResponse.EmployerName);
            result.AccountProviderLegalEntities.Count.Should().Be(filteredLegalEntities.Count());
        }

        foreach (var entity in filteredLegalEntities)
        {
            result.AccountProviderLegalEntities.Any(x =>
                x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                x.EmployerAccountName == entity.AccountName &&
                x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId).Should().BeTrue();
        }
    }

    public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
    {
        result.AccountProviderLegalEntities.Should().BeEmpty();
    }
}