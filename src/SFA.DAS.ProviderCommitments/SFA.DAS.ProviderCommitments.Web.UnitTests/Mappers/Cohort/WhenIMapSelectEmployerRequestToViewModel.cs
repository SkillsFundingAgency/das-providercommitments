using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

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

            fixture.Assert_SortIsAppliedCorrectlyForEmployerName(result, reverseSort);
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

            fixture.Assert_SortIsAppliedCorrectlyForEmployerAccountName(result, reverseSort);
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

            fixture.Assert_ListOfEmployersIsEmpty(result);
        }


        public class SelectEmployerViewModelMapperFixture
        {
            private readonly SelectEmployerViewModelMapper _sut;
            private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
            private SelectEmployerRequest _request;
            private GetAccountProviderLegalEntitiesWithPermissionResponse _apiResponse;

            public SelectEmployerViewModelMapperFixture()
            {
                const long providerId = 123;
                _request = new SelectEmployerRequest { ProviderId = providerId };
                _apiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
                {
                    AccountProviderLegalEntities = new List<AccountProviderLegalEntityDto>
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

                _providerRelationshipsApiClientMock = new Mock<IProviderRelationshipsApiClient>();
                _providerRelationshipsApiClientMock
                    .Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(
                        It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(),
                        CancellationToken.None))
                    .ReturnsAsync(() => _apiResponse);

                _sut = new SelectEmployerViewModelMapper(_providerRelationshipsApiClientMock.Object);
            }

            public SelectEmployerViewModelMapperFixture AddListOfAccountProviderLegalEntities()
            {
                _apiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
                {
                    AccountProviderLegalEntities = new List<AccountProviderLegalEntityDto>
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
                _providerRelationshipsApiClientMock
                    .Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(
                        It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(),
                        CancellationToken.None))
                    .ReturnsAsync(new GetAccountProviderLegalEntitiesWithPermissionResponse());

                return this;
            }

            public void Verify_ProviderRelationshipsApiClientWasCalled_Once()
            {
                _providerRelationshipsApiClientMock.Verify(x => x.GetAccountProviderLegalEntitiesWithPermission(
                    It.Is<GetAccountProviderLegalEntitiesWithPermissionRequest>(y =>
                        y.Ukprn == _request.ProviderId &&
                        y.Operation == Operation.CreateCohort), CancellationToken.None), Times.Once);
            }

            public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(_apiResponse.AccountProviderLegalEntities.Count()));

                foreach (var entity in _apiResponse.AccountProviderLegalEntities)
                {
                    Assert.That(result.AccountProviderLegalEntities.Any(x =>
                        x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                        x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                        x.EmployerAccountName == entity.AccountName &&
                        x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId), Is.True);
                }
            }

            public void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(0));
            }

            internal void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
            {
                if (reverseSort)
                {
                    Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("CTestAccountLegalEntityName"));
                    Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName, Is.EqualTo("BTestAccountLegalEntityName"));
                    Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
                }
                else
                {
                    Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
                    Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName, Is.EqualTo("BTestAccountLegalEntityName"));
                    Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName, Is.EqualTo("CTestAccountLegalEntityName"));
                }
            }

            internal void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
            {
                if (reverseSort)
                {
                    Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("CTestAccountName"));
                    Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountName, Is.EqualTo("BTestAccountName"));
                    Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountName, Is.EqualTo("ATestAccountName"));
                }
                else
                {
                    Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("ATestAccountName"));
                    Assert.That(result.AccountProviderLegalEntities[1].EmployerAccountName, Is.EqualTo("BTestAccountName"));
                    Assert.That(result.AccountProviderLegalEntities[2].EmployerAccountName, Is.EqualTo("CTestAccountName"));
                }
            }

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo("ATestAccountName"));
            }

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo("ATestAccountLegalEntityName"));
            }
        }
    }
}

