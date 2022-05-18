using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class WhenIMapSelectEmployerRequestToViewModel
    {
        [Test]
        public async Task ThenCallsProviderRelationshipsApiClient()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(456);

            var result = await fixture.Map(new SelectEmployerRequest { ProviderId = 456 } );

            fixture.Verify_ProviderRelationshipsApiClientWasCalled_Once(456);
        }

        [Test]
        public async Task ThenCallsCommitmentsApiClient()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(456);

            await fixture.Map(new SelectEmployerRequest { ApprenticeshipId = 123 });

            fixture.Verify_CommitmentsApiClientWasCalled_Once(123);
        }

        [Test]
        public async Task ThenCorrectlyMapsApiResponseToViewModel()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(456);

            var result = await fixture.Map(new SelectEmployerRequest());

            fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenSortIsAppliedCorrectlyForEmployerName(bool reverseSort)
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(456);

            var result = await fixture.Map(new SelectEmployerRequest
                {
                    ProviderId = 123,
                    SortField = SelectEmployerFilterModel.EmployerAccountLegalEntityNameConst,
                    ReverseSort = reverseSort
                });

            fixture.Assert_SortIsAppliedCorrectlyForEmployerName(result, reverseSort);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenSortIsAppliedCorrectlyForEmployerAccountName(bool reverseSort)
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(456);
                
            var result = await fixture.Map(new SelectEmployerRequest
                {
                    ProviderId = 123,
                    SortField = SelectEmployerFilterModel.EmployerAccountNameConst,
                    ReverseSort = reverseSort
                });

            fixture.Assert_SortIsAppliedCorrectlyForEmployerAccountName(result, reverseSort);
        }

        [Test]
        public async Task ThenFilterIsAppliedCorrectlyForEmployerAccountName()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(458);

            var result = await fixture.Map(new SelectEmployerRequest
                {
                    ProviderId = 123,
                    SearchTerm = "atestaccountname"
                });

            fixture.Assert_FilterIsAppliedCorrectlyForEmployerAccountName(result, "ATestAccountName");
        }


        [Test]
        public async Task ThenFilterIsAppliedCorrectlyForEmployerName()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithListOfAccountProviderLegalEntities(new List<(string Prefix, long AccountLegalEntityId)> { ("A", 456), ("B", 457), ("C", 458) })
                .WithApprenticeship(458);

            var result = await fixture.Map(new SelectEmployerRequest
                {
                    ProviderId = 123,
                    SearchTerm = "atestaccountlegal"
                });

            fixture.Assert_FilterIsAppliedCorrectlyForEmployerName(result, "ATestAccountLegalEntityName");
        }

        [Test]
        public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
        {
            var fixture = new SelectEmployerViewModelMapperFixture()
                .WithNoMatchingEmployers()
                .WithApprenticeship(456);

            var result = await fixture.Map(new SelectEmployerRequest
            {
                ProviderId = 123,
                SearchTerm = "atestaccountlegal"
            });

            fixture.Assert_ListOfEmployersIsEmpty(result);
        }

        public class SelectEmployerViewModelMapperFixture
        {
            private readonly SelectEmployerViewModelMapper _sut;
            private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
            private readonly Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
            private SelectEmployerRequest _request;
            private readonly long _providerId;
            
            private GetAccountProviderLegalEntitiesWithPermissionResponse _getAccountsApiResponse;
            private GetApprenticeshipResponse _getApprenticeshipApiResponse;

            public SelectEmployerViewModelMapperFixture()
            {
                _providerId = 123;
                _request = new SelectEmployerRequest { ProviderId = _providerId };
                _getAccountsApiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
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
                    .ReturnsAsync(() => _getAccountsApiResponse);

                _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
                _commitmentsApiClientMock
                    .Setup(x => x.GetApprenticeship(It.IsAny<long>(), CancellationToken.None))
                    .ReturnsAsync(() => _getApprenticeshipApiResponse);

                _sut = new SelectEmployerViewModelMapper(_providerRelationshipsApiClientMock.Object, _commitmentsApiClientMock.Object);
            }

            public SelectEmployerViewModelMapperFixture WithListOfAccountProviderLegalEntities(List<(string Prefix, long AccountLegalEntityId)> legalEntities)
            {
                _getAccountsApiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
                {
                    AccountProviderLegalEntities = legalEntities
                        .Select(x => new AccountProviderLegalEntityDto
                        {
                            AccountId = 123,
                            AccountLegalEntityPublicHashedId = $"{x.Prefix}DSFF23",
                            AccountLegalEntityName = $"{x.Prefix}TestAccountLegalEntityName",
                            AccountPublicHashedId = $"{x.Prefix}DFKFK66",
                            AccountName = $"{x.Prefix}TestAccountName",
                            AccountLegalEntityId = x.AccountLegalEntityId,
                            AccountProviderId = 234
                        })
                        .ToList()
                };

                return this;
            }

            public SelectEmployerViewModelMapperFixture WithApprenticeship(long accountLegalEntityId)
            {
                _getApprenticeshipApiResponse = new GetApprenticeshipResponse
                {
                    AccountLegalEntityId = accountLegalEntityId
                };

                return this;
            }

            internal SelectEmployerViewModelMapperFixture WithRequest(SelectEmployerRequest selectEmployerRequest)
            {
                _request = selectEmployerRequest;
                return this;
            }

            public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest selectEmployerRequest) => await _sut.Map(selectEmployerRequest);


            public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
            {
                _providerRelationshipsApiClientMock
                    .Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(
                        It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(),
                        CancellationToken.None))
                    .ReturnsAsync(new GetAccountProviderLegalEntitiesWithPermissionResponse());

                return this;
            }

            public void Verify_ProviderRelationshipsApiClientWasCalled_Once(long providerId)
            {
                _providerRelationshipsApiClientMock.Verify(x => x.GetAccountProviderLegalEntitiesWithPermission(
                    It.Is<GetAccountProviderLegalEntitiesWithPermissionRequest>(y =>
                        y.Ukprn == providerId &&
                        y.Operation == Operation.CreateCohort), CancellationToken.None), Times.Once);
            }

            public void Verify_CommitmentsApiClientWasCalled_Once(long apprenticeshipId)
            {
                _commitmentsApiClientMock.Verify(x => x.GetApprenticeship(
                    It.Is<long>(y => y == apprenticeshipId), CancellationToken.None), Times.Once);
            }

            public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
            {
                var accountProviderLegalEntities = _getAccountsApiResponse.AccountProviderLegalEntities
                    .Where(x => x.AccountLegalEntityId != _getApprenticeshipApiResponse.AccountLegalEntityId)
                    .Select(x => new AccountProviderLegalEntityViewModel
                        {
                            EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                            EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                            EmployerAccountName = x.AccountName,
                            EmployerAccountPublicHashedId = x.AccountPublicHashedId
                        })
                    .ToList();

                accountProviderLegalEntities.Should().BeEquivalentTo(result.AccountProviderLegalEntities);
            }

            public void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
            {
                Assert.AreEqual(0, result.AccountProviderLegalEntities.Count());
            }

            internal void Assert_SortIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, bool reverseSort)
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

            internal void Assert_SortIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, bool reverseSort)
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

            internal void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, string employerAccountName)
            {
                Assert.AreEqual(1, result.AccountProviderLegalEntities.Count);
                Assert.AreEqual(employerAccountName, result.AccountProviderLegalEntities[0].EmployerAccountName);
            }

            internal void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, string employerName)
            {
                Assert.AreEqual(1, result.AccountProviderLegalEntities.Count);
                Assert.AreEqual(employerName, result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName);
            }
        }
    }
}

