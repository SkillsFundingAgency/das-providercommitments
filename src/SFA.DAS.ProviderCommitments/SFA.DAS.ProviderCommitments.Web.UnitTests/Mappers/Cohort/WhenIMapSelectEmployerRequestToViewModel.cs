using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
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
                    .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>(), It.IsAny<string>(), ""))
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
                    .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>(), It.IsAny<string>(), ""))
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
                    .Setup(x => x.GetProviderAccountLegalEntities(It.IsAny<int>(), It.IsAny<string>(), ""))
                    .ReturnsAsync((GetProviderAccountLegalEntitiesResponse)null);

                return this;
            }

            public void Verify_ProviderRelationshipsApiClientWasCalled_Once()
            {
                _approvalsOuterApiClientMock.Verify(
                    x => x.GetProviderAccountLegalEntities((int)_request.ProviderId, Operation.CreateCohort.ToString(),
                        It.IsAny<string>()), Times.Once);
            }

            public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities, Has.Count.EqualTo(_apiResponse.AccountProviderLegalEntities.Count()));

                foreach (var entity in _apiResponse.AccountProviderLegalEntities)
                {
                    Assert.That(result.AccountProviderLegalEntities.Any(x =>
                        x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                        x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                        x.EmployerAccountName == entity.AccountName &&
                        x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId), Is.True);
                }
            }

            public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities, Is.Empty);
            }

            internal static void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
            {
                if (reverseSort)
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("CTestAccountLegalEntityName"));
                        Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName, Is.EqualTo("BTestAccountLegalEntityName"));
                        Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
                    });
                }
                else
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
                        Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName, Is.EqualTo("BTestAccountLegalEntityName"));
                        Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName, Is.EqualTo("CTestAccountLegalEntityName"));
                    });
                }
            }

            internal static void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
            {
                if (reverseSort)
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("CTestAccountName"));
                        Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountName, Is.EqualTo("BTestAccountName"));
                        Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountName, Is.EqualTo("ATestAccountName"));
                    });
                }
                else
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("ATestAccountName"));
                        Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountName, Is.EqualTo("BTestAccountName"));
                        Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountName, Is.EqualTo("CTestAccountName"));
                    });
                }
            }

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities, Has.Count.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("ATestAccountName"));
            }

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities, Has.Count.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
            }
        }
    }
}

