using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.SelectEmployerRequestToViewModelTests
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

        [Test]
        public async Task ThenCallsLinkGeneratorForBackLink()
        {
            var fixture = new SelectEmployerViewModelMapperFixture();

            await fixture.Act();

            fixture.Verify_LinkGeneratorCalledForBackLink();
        }

        [Test]
        public async Task ThenGeneratesBackLink()
        {
            var fixture = new SelectEmployerViewModelMapperFixture();

            var result = await fixture.Act();

            fixture.Assert_BackLinkIsGenerated(result);
        }
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClientMock;
        private readonly Mock<ILinkGenerator> _linkGeneratorMock;
        private readonly SelectEmployerRequest _request;
        private readonly long _providerId;
        private readonly string _TestBackLink;
        private readonly GetAccountProviderLegalEntitiesWithPermissionResponse _apiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            _providerId = 123;
            _TestBackLink = "testBackLink.com";
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
                .ReturnsAsync(_apiResponse);

            _linkGeneratorMock = new Mock<ILinkGenerator>();
            _linkGeneratorMock
                .Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                .Returns(_TestBackLink);
            _sut = new SelectEmployerViewModelMapper(_providerRelationshipsApiClientMock.Object, _linkGeneratorMock.Object);
        }

        public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);

        public void Verify_ProviderRelationshipsApiClientWasCalled_Once()
        {
            _providerRelationshipsApiClientMock.Verify(x => x.GetAccountProviderLegalEntitiesWithPermission(
                It.Is<GetAccountProviderLegalEntitiesWithPermissionRequest>(y => 
                    y.Ukprn == _request.ProviderId &&
                    y.Operation == Operation.CreateCohort), CancellationToken.None), Times.Once);
        }

        public void Verify_LinkGeneratorCalledForBackLink()
        {
            _linkGeneratorMock.Verify(x => x.ProviderApprenticeshipServiceLink("account"));
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

        public void Assert_BackLinkIsGenerated(SelectEmployerViewModel result)
        {
            Assert.NotNull(result.BackLink);
            Assert.AreEqual(_TestBackLink, result.BackLink);
        }



    }
}

