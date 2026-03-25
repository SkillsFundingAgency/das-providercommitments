using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class WhenIMapSelectEmployerRequestToViewModel
{
    [Test]
    public async Task ThenCallsGetSelectEmployerApi()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();

        // Act
        await fixture.Act();

        // Assert
        fixture.Verify_GetSelectNewEmployerWasCalled_Once();
    }

    [Test]
    public async Task ThenCorrectlyMapsApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        // Act
        var result = await fixture.Act();

        fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenSortIsAppliedCorrectlyForEmployerName(bool reverseSort)
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SortField = SelectEmployerFilterModel.EmployerAccountLegalEntityNameConst,
            ReverseSort = reverseSort
        });

        SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerName(result, reverseSort);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenSortIsAppliedCorrectlyForEmployerAccountName(bool reverseSort)
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SortField = SelectEmployerFilterModel.EmployerAccountNameConst,
            ReverseSort = reverseSort
        });

        SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerAccountName(result, reverseSort);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenSortIsAppliedCorrectlyForAgreementId(bool reverseSort)
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SortField = SelectEmployerFilterModel.EmployerAccountNameConst,
            ReverseSort = reverseSort
        });

        SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForAgreementId(result, reverseSort);
    }

    [Test]
    public async Task ThenFilterIsAppliedCorrectlyForEmployerAccountName()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "account"
        });

        SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerAccountName(result, "TestAccountName");
    }

    [Test]
    public async Task ThenFilterIsAppliedCorrectlyForEmployerName()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "testaccountlegal"
        });

        SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerName(result, "TestAccountLegalEntityName");
    }

    [Test]
    public async Task ThenFilterIsAppliedCorrectlyForAgreementId()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "DSFF23"
        });

        SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForAgreementId(result, "DSFF23");
    }

    [Test]
    public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture().
            WithNoMatchingEmployers().
            WithApprenticeship(10);

        var result = await fixture.Map(new SelectEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "atestaccountlegal"
        });

        SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
        private SelectEmployerRequest _request;
        private GetSelectEmployerResponse _apiResponse;
        private GetApprenticeshipResponse _getApprenticeshipApiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            const long providerId = 101;
            _request = new SelectEmployerRequest { ProviderId = providerId };

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

            _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectNewEmployer(It.IsAny<GetSelectNewEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            _sut = new SelectEmployerViewModelMapper(_approvalsOuterApiClientMock.Object);
        }

        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest selectEmployerRequest) => await _sut.Map(selectEmployerRequest);

        public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
        {
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectNewEmployer(It.IsAny<GetSelectNewEmployerRequest>()))
                .ReturnsAsync(new GetSelectEmployerResponse() { AccountProviderLegalEntities = null });

            return this;
        }

        public void Verify_ProviderRelationshipsApiClientWasCalled_Once(long providerId)
        {
            _approvalsOuterApiClientMock.Verify(
                x => x.GetProviderAccountLegalEntities((int)providerId), Times.Once);
        }

        public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(_apiResponse.AccountProviderLegalEntities.Count);

            foreach (var entity in _apiResponse.AccountProviderLegalEntities)
            {
                result.AccountProviderLegalEntities.Any(x =>
                    x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                    x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                    x.EmployerAccountName == entity.AccountName &&
                    x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId &&
                    x.AccountHashedId == entity.AccountHashedId).Should().BeTrue();
            }
        }

        public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Should().BeEmpty();
        }

        public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);

        internal static void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                result.AccountProviderLegalEntities.Select(x => x.EmployerAccountLegalEntityName).Should().BeInDescendingOrder();
            }
            else
            {
                result.AccountProviderLegalEntities.Select(x => x.EmployerAccountLegalEntityName).Should().BeInAscendingOrder();
            }
        }

        internal static void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                result.AccountProviderLegalEntities
                    .Select(x => x.EmployerAccountName).Should().BeInDescendingOrder();
            }
            else
            {
                result.AccountProviderLegalEntities.Select(x => x.EmployerAccountName).Should().BeInAscendingOrder();
            }
        }

        internal static void Assert_SortIsAppliedCorrectlyForAgreementId(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                result.AccountProviderLegalEntities
                    .Select(x => x.EmployerAccountLegalEntityPublicHashedId).Should().BeInDescendingOrder();
            }
            else
            {
                result.AccountProviderLegalEntities.Select(x => x.EmployerAccountLegalEntityPublicHashedId).Should().BeInAscendingOrder();
            }
        }

        internal static void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, string accountName)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(1);
            result.AccountProviderLegalEntities[0].EmployerAccountName.Should().Be(accountName);
        }

        internal static void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, string legalEntityName)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(1);
            result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName.Should().Be(legalEntityName);
        }

        internal static void Assert_FilterIsAppliedCorrectlyForAgreementId(SelectEmployerViewModel result, string agreementId)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(1);
            result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityPublicHashedId.Should().Be(agreementId);
        }

        public void Verify_GetSelectNewEmployerWasCalled_Once()
        {
            _approvalsOuterApiClientMock.Verify(
                x => x.GetSelectNewEmployer(It.Is<GetSelectNewEmployerRequest>(r =>
                    r.GetUrl.Contains(_request.ProviderId.ToString()))), Times.Once);
        }

        public SelectEmployerViewModelMapperFixture WithApprenticeship(long accountLegalEntityId)
        {
            _getApprenticeshipApiResponse = new GetApprenticeshipResponse
            {
                AccountLegalEntityId = accountLegalEntityId
            };

            return this;
        }
    }
}