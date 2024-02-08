using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

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

            await fixture.Map(new SelectEmployerRequest { ProviderId = 456 } );

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

            SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerName(result, reverseSort);
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

            SelectEmployerViewModelMapperFixture.Assert_SortIsAppliedCorrectlyForEmployerAccountName(result, reverseSort);
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

            SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerAccountName(result, "ATestAccountName");
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

            SelectEmployerViewModelMapperFixture.Assert_FilterIsAppliedCorrectlyForEmployerName(result, "ATestAccountLegalEntityName");
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

            SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
        }

        public class SelectEmployerViewModelMapperFixture
        {
            private readonly SelectEmployerViewModelMapper _sut;
            private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
            private readonly Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
            private GetAccountProviderLegalEntitiesWithPermissionResponse _getAccountsApiResponse;
            private GetApprenticeshipResponse _getApprenticeshipApiResponse;

            public SelectEmployerViewModelMapperFixture()
            {
                _getAccountsApiResponse = new GetAccountProviderLegalEntitiesWithPermissionResponse
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

            public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(0));
            }

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

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerAccountName(SelectEmployerViewModel result, string employerAccountName)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountName, Is.EqualTo(employerAccountName));
            }

            internal static void Assert_FilterIsAppliedCorrectlyForEmployerName(SelectEmployerViewModel result, string employerName)
            {
                Assert.That(result.AccountProviderLegalEntities.Count, Is.EqualTo(1));
                Assert.That(result.AccountProviderLegalEntities[0].EmployerAccountLegalEntityName, Is.EqualTo(employerName));
            }
        }
    }
}

