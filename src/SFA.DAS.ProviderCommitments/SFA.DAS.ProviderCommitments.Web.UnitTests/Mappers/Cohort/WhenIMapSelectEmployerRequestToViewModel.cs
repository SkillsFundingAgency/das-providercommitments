using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
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

            fixture.Assert_FilterIsAppliedCorrectlyForEmployerAccountName(result);
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

            fixture.Assert_FilterIsAppliedCorrectlyForEmployerName(result);
        }

        [Test]
        public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
        {
            var fixture = new SelectEmployerViewModelMapperFixture().WithNoMatchingEmployers();

            var result = await fixture.Act();

            fixture.Assert_ListOfEmployersIsEmpty(result);
        }
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
        private SelectEmployerRequest _request;
        private readonly long _providerId;
        private GetAccountProviderLegalEntitiesWithPermissionResponse _apiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            _providerId = 123;
            _request = new SelectEmployerRequest {ProviderId = _providerId};
            _apiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityDto>
                {
                    new AccountProviderLegalEntityDto
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
                    new AccountProviderLegalEntityDto
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "ADSFF23",
                        AccountLegalEntityName = "ATestAccountLegalEntityName",
                        AccountPublicHashedId = "ADFKFK66",
                        AccountName = "CTestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    },
                     new AccountProviderLegalEntityDto
                    {
                        AccountId = 123,
                        AccountLegalEntityPublicHashedId = "BDSFF23",
                        AccountLegalEntityName = "BTestAccountLegalEntityName",
                        AccountPublicHashedId = "BDFKFK66",
                        AccountName = "BTestAccountName",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234
                    },
                      new AccountProviderLegalEntityDto
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
            Assert.AreEqual(_apiResponse.AccountProviderLegalEntities.Count(), result.AccountProviderLegalEntities.Count());

            foreach (var entity in _apiResponse.AccountProviderLegalEntities)
            {
                Assert.True(result.AccountProviderLegalEntities.Any(x => 
                    x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                    x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                    x.EmployerAccountName == entity.AccountName &&
                    x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId));
            }
        }

        public void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
        {
            Assert.AreEqual(0, result.AccountProviderLegalEntities.Count());
        }

        internal void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                Assert.AreEqual("CTestAccountLegalEntityName", result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName);
                Assert.AreEqual("BTestAccountLegalEntityName", result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName);
                Assert.AreEqual("ATestAccountLegalEntityName", result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName);
            }
            else
            {
                Assert.AreEqual("ATestAccountLegalEntityName", result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName);
                Assert.AreEqual("BTestAccountLegalEntityName", result.AccountProviderLegalEntities[1].EmployerAccountLegalEntityName);
                Assert.AreEqual("CTestAccountLegalEntityName", result.AccountProviderLegalEntities[2].EmployerAccountLegalEntityName);
            }
        }

        internal void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
        {
            if (reverseSort)
            {
                Assert.AreEqual("CTestAccountName", result.AccountProviderLegalEntities[0].EmployerAccountName);
                Assert.AreEqual("BTestAccountName", result.AccountProviderLegalEntities[1].EmployerAccountName);
                Assert.AreEqual("ATestAccountName", result.AccountProviderLegalEntities[2].EmployerAccountName);
            }
            else
            {
                Assert.AreEqual("ATestAccountName", result.AccountProviderLegalEntities[0].EmployerAccountName);
                Assert.AreEqual("BTestAccountName", result.AccountProviderLegalEntities[1].EmployerAccountName);
                Assert.AreEqual("CTestAccountName", result.AccountProviderLegalEntities[2].EmployerAccountName);
            }
        }

        internal void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result)
        {
            Assert.AreEqual(1, result.AccountProviderLegalEntities.Count);
            Assert.AreEqual("ATestAccountName", result.AccountProviderLegalEntities[0].EmployerAccountName);
        }

        internal void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result)
        {
            Assert.AreEqual(1, result.AccountProviderLegalEntities.Count);
            Assert.AreEqual("ATestAccountLegalEntityName", result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName);
        }
    }
}

