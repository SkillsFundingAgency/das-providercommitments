using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class SelectEmployerViewModelMapperTests
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

            SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
        }
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClientMock;
        private readonly SelectEmployerRequest _request;
        private readonly long _accountLegalEntityId;
        private readonly long _apprenticeshipId;
        private readonly GetProviderAccountLegalEntitiesResponse _apiResponse;

        public GetApprenticeshipResponse GetApprenticeshipApiResponse { get; }

        public SelectEmployerViewModelMapperFixture()
        {
            const long providerId = 123;
            _accountLegalEntityId = 457;
            _apprenticeshipId = 1;
            _request = new SelectEmployerRequest { ProviderId = providerId, ApprenticeshipId = _apprenticeshipId };
            _apiResponse = new GetProviderAccountLegalEntitiesResponse
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
                    },
                     new()
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

            GetApprenticeshipApiResponse = new GetApprenticeshipResponse
            {
                AccountLegalEntityId = _accountLegalEntityId,
                EmployerName = "TestName"
            };

            _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
            _approvalsOuterApiClientMock
                .Setup(x => x.GetProviderAccountLegalEntities(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_apiResponse);

            _commitmentApiClientMock = new Mock<ICommitmentsApiClient>();

            _commitmentApiClientMock.Setup(x => x.GetApprenticeship(_apprenticeshipId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetApprenticeshipApiResponse);

            _sut = new SelectEmployerViewModelMapper(_approvalsOuterApiClientMock.Object, _commitmentApiClientMock.Object);
        }

        public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);


        public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
        {
            _approvalsOuterApiClientMock
                .Setup(x => x.GetProviderAccountLegalEntities(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetProviderAccountLegalEntitiesResponse());

            return this;
        }

        public void Verify_ProviderRelationshipsApiClientWasCalled_Once()
        {
            _approvalsOuterApiClientMock.Verify(x => x.GetProviderAccountLegalEntities(
                (int)_request.ProviderId, Operation.CreateCohort.ToString(), It.IsAny<string>()), Times.Once);
        }

        public void Verify_CommitmentApiClientWasCalled_Once()
        {
            _commitmentApiClientMock.Verify(x => x.GetApprenticeship(_apprenticeshipId, CancellationToken.None), Times.Once);
        }

        public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
        {
            var filteredLegalEntities = _apiResponse.AccountProviderLegalEntities.Where(x => x.AccountLegalEntityId != _accountLegalEntityId);
            Assert.Multiple(() =>
            {
                Assert.That(result.LegalEntityName, Is.EqualTo(GetApprenticeshipApiResponse.EmployerName));
                Assert.That(result.AccountProviderLegalEntities, Has.Count.EqualTo(filteredLegalEntities.Count()));
            });

            foreach (var entity in filteredLegalEntities)
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
    }
}

