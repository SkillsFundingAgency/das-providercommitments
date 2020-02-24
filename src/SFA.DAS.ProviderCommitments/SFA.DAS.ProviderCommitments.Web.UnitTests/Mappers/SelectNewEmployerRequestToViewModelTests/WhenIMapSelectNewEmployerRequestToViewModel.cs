using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.SelectNewEmployerRequestToViewModelTests
{
    [TestFixture]
    public class WhenIMapSelectNewEmployerRequestToViewModel
    {
        [Test]
        public async Task ThenCallsProviderRelationshipsApiClient()
        {
            var fixture = new SelectEmployerViewModelMapperFixture();

            await fixture.Act();

            fixture.Verify_ProviderRelationshipsApiClientWasCalled_Once();
        }

        [Test]
        public async Task ThenCallsCommitmentsApiClient()
        {
            var fixture = new SelectEmployerViewModelMapperFixture();

            await fixture.Act();

            fixture.Verify_CommitmentApiClientWasCalled_Once();
        }

        [Test]
        public async Task ThenCorrectlyMapsApiResponseToViewModel()
        {
            var fixture = new SelectEmployerViewModelMapperFixture();

            var result = await fixture.Act();

            fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
        }

        [Test]
        public async Task ThenDontReturnTheExistingEmployer()
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

            fixture.Assert_ListOfEmployersIsEmpty(result);
        }
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly Web.Mappers.Apprentice.SelectNewEmployerViewModelMapper _sut;
        private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClientMock;
        private readonly Requests.Apprentice.SelectNewEmployerRequest _request;
        private readonly long _providerId;
        private readonly long _accountLegalEntityId;
        private readonly long _apprenticeshipId;
        private readonly GetAccountProviderLegalEntitiesWithPermissionResponse _apiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            _providerId = 123;
            _accountLegalEntityId = 457;
            _apprenticeshipId = 1;
        _request = new Requests.Apprentice.SelectNewEmployerRequest { ProviderId = _providerId, ApprenticeshipId = _apprenticeshipId};
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
                    },
                     new AccountProviderLegalEntityDto
                    {
                        AccountId = 124,
                        AccountLegalEntityPublicHashedId = "DSFF24",
                        AccountLegalEntityName = "TestAccountLegalEntityName2",
                        AccountPublicHashedId = "DFKFK67",
                        AccountName = "TestAccountNam2",
                        AccountLegalEntityId = _accountLegalEntityId,
                        AccountProviderId = 235
                    }
                }
            };

            _providerRelationshipsApiClientMock = new Mock<IProviderRelationshipsApiClient>();
            _providerRelationshipsApiClientMock
                .Setup(x => x.GetAccountProviderLegalEntitiesWithPermission(
                    It.IsAny<GetAccountProviderLegalEntitiesWithPermissionRequest>(),
                    CancellationToken.None))
                .ReturnsAsync(_apiResponse);

            _commitmentApiClientMock = new Mock<ICommitmentsApiClient>();

            _commitmentApiClientMock.Setup(x => x.GetApprenticeship(_apprenticeshipId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommitmentsV2.Api.Types.Responses.GetApprenticeshipResponse
                {
                    AccountLegalEntityId = _accountLegalEntityId
                });

            _sut = new Web.Mappers.Apprentice.SelectNewEmployerViewModelMapper(_providerRelationshipsApiClientMock.Object, _commitmentApiClientMock.Object);
        }

        public async Task<SelectNewEmployerViewModel> Act() => await _sut.Map(_request);


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

        public void Verify_CommitmentApiClientWasCalled_Once()
        {
            _commitmentApiClientMock.Verify(x => x.GetApprenticeship(_apprenticeshipId, CancellationToken.None), Times.Once);
        }

        public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectNewEmployerViewModel result)
        {
            var filteredLegalEntities = _apiResponse.AccountProviderLegalEntities.Where(x => x.AccountLegalEntityId != _accountLegalEntityId);
            Assert.AreEqual(filteredLegalEntities.Count(), result.AccountProviderLegalEntities.Count());

            foreach (var entity in filteredLegalEntities)
            {
                Assert.True(result.AccountProviderLegalEntities.Any(x => 
                    x.EmployerAccountLegalEntityName == entity.AccountLegalEntityName &&
                    x.EmployerAccountLegalEntityPublicHashedId == entity.AccountLegalEntityPublicHashedId &&
                    x.EmployerAccountName == entity.AccountName &&
                    x.EmployerAccountPublicHashedId == entity.AccountPublicHashedId));
            }
        }

        public void Assert_ListOfEmployersIsEmpty(SelectNewEmployerViewModel result)
        {
            Assert.AreEqual(0, result.AccountProviderLegalEntities.Count());
        }
    }
}

