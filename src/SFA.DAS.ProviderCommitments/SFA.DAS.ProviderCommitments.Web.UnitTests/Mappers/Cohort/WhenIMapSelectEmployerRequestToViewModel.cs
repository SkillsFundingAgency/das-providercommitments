using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapSelectEmployerRequestToViewModel
{
    [Test]
    public async Task ThenCallsProviderRelationshipsApiClient()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        await fixture.Act();

        fixture.Verify_ProviderRelationshipsApiClientWasCalled_Once();
    }

    [Test]
    public async Task ThenCorrectlyMapsApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();

        var result = await fixture.Act();

        fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenSortIsAppliedCorrectlyForEmployerName(bool reverseSort)
    {
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.AddListOfAccountProviderLegalEntities()
            .WithRequest(new SelectEmployerRequest
            {
                ProviderId = 123,
                SortField = SelectEmployerFilterModel.EmployerAccountLegalEntityNameConst,
                ReverseSort = reverseSort
            });

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerName(result, reverseSort);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenSortIsAppliedCorrectlyForEmployerAccountName(bool reverseSort)
    {
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.AddListOfAccountProviderLegalEntities()
            .WithRequest(new SelectEmployerRequest
            {
                ProviderId = 123,
                SortField = SelectEmployerFilterModel.EmployerAccountNameConst,
                ReverseSort = reverseSort
            });

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerAccountName(result, reverseSort);
    }

    [Test]
    public async Task ThenFilterIsAppliedCorrectlyForEmployerAccountName()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.AddListOfAccountProviderLegalEntities()
            .WithRequest(new SelectEmployerRequest
            {
                ProviderId = 123,
                SearchTerm = "atestaccountname"
            });

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerAccountName(result);
    }


    [Test]
    public async Task ThenFilterIsAppliedCorrectlyForEmployerName()
    {
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.AddListOfAccountProviderLegalEntities()
            .WithRequest(new SelectEmployerRequest
            {
                ProviderId = 123,
                SearchTerm = "atestaccountlegal"
            });

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerName(result);
    }

    [Test]
    public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
    {
        var fixture = new SelectEmployerViewModelMapperFixture().WithNoMatchingEmployers();

        var result = await fixture.Act();

        SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
    }


    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
        private SelectEmployerRequest _request;
        private GetProviderAccountLegalEntitiesResponse _apiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            const long providerId = 123;
            _request = new SelectEmployerRequest { ProviderId = providerId };
            _apiResponse = new GetProviderAccountLegalEntitiesResponse()
            {
                AccountProviderLegalEntities = new List<GetProviderAccountLegalEntityItem>
                {
                    new()
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "DSFF23",
                        AccountLegalEntityName = "TestAccountLegalEntityName",
                        AccountPublicHashedId = "DFKFK66",
                        AccountName = "TestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    }
                }
            };

            _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
            _approvalsOuterApiClientMock
                .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>()))
                .ReturnsAsync(_apiResponse);

            _sut = new SelectEmployerViewModelMapper(_approvalsOuterApiClientMock.Object);
        }

        public SelectEmployerViewModelMapperFixture AddListOfAccountProviderLegalEntities()
        {
            _apiResponse = new GetProviderAccountLegalEntitiesResponse
            {
                AccountProviderLegalEntities = new List<GetProviderAccountLegalEntityItem>
                {
                    new()
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "ADSFF23",
                        AccountLegalEntityName = "ATestAccountLegalEntityName",
                        AccountPublicHashedId = "ADFKFK66",
                        AccountName = "CTestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    },
                    new()
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "BDSFF23",
                        AccountLegalEntityName = "BTestAccountLegalEntityName",
                        AccountPublicHashedId = "BDFKFK66",
                        AccountName = "BTestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    },
                    new()
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "CDSFF23",
                        AccountLegalEntityName = "CTestAccountLegalEntityName",
                        AccountPublicHashedId = "CDFKFK66",
                        AccountName = "ATestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    }
                }
            };
            _approvalsOuterApiClientMock
                .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>()))
                .ReturnsAsync(_apiResponse);

            return this;
        }

        internal SelectEmployerViewModelMapperFixture WithRequest(SelectEmployerRequest selectEmployerRequest)
        {
            _request = selectEmployerRequest;
            return this;
        }

        public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);


        public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
        {
            _approvalsOuterApiClientMock
                .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>()))
                .ReturnsAsync((GetProviderAccountLegalEntitiesResponse)null);

            return this;
        }

        public void Verify_ProviderRelationshipsApiClientWasCalled_Once()
        {
            _approvalsOuterApiClientMock.Verify(
                x => x.GetProviderAccountLegalEntities((int)_request.ProviderId), Times.Once);
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
                    x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId).Should().BeTrue();
            }
        }

        public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Should().BeEmpty();
        }

        internal static void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                using (new AssertionScope())
                {
                    result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName.Should().Be("CTestAccountLegalEntityName");
                    result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName.Should().Be("BTestAccountLegalEntityName");
                    result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName.Should().Be("ATestAccountLegalEntityName");
                };
            }
            else
            {
                using (new AssertionScope())
                {
                    result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName.Should().Be("ATestAccountLegalEntityName");
                    result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName.Should().Be("BTestAccountLegalEntityName");
                    result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName.Should().Be("CTestAccountLegalEntityName");
                };
            }
        }

        internal static void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                using (new AssertionScope())
                {
                    result.AccountProviderLegalEntities[0].EmployerAccountName.Should().Be("CTestAccountName");
                    result.AccountProviderLegalEntities[1].EmployerAccountName.Should().Be("BTestAccountName");
                    result.AccountProviderLegalEntities[2].EmployerAccountName.Should().Be("ATestAccountName");
                }
            }
            else
            {
                using (new AssertionScope())
                {
                    result.AccountProviderLegalEntities[0].EmployerAccountName.Should().Be("ATestAccountName");
                    result.AccountProviderLegalEntities[1].EmployerAccountName.Should().Be("BTestAccountName");
                    result.AccountProviderLegalEntities[2].EmployerAccountName.Should().Be("CTestAccountName");
                }
            }
        }

        internal static void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(1);
            result.AccountProviderLegalEntities[0].EmployerAccountName.Should().Be("ATestAccountName");
        }

        internal static void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(1);
            result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName.Should().Be("ATestAccountLegalEntityName");
        }
    }
}