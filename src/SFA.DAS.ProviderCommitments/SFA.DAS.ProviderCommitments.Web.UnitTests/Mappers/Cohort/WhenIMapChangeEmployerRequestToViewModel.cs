using System;
using System.Linq;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapChangeEmployerRequestToViewModel
{
    [Test]
    public async Task ThenCallsGetSelectEmployerApi()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();

        await fixture.Act();

        fixture.Verify_GetSelectEmployerWasCalled_Once();
    }

    [Test]
    public async Task ThenCorrectlyMapsApiResponseToViewModel()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();

        var result = await fixture.Act();

        fixture.Assert_ChangeEmployerViewModelCorrectlyMapped(result);
    }

    [Test]
    public async Task ThenCorrectlyMapsLevyStatus_WhenLevy()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("Levy");

        var result = await fixture.Act();

        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.Levy);
    }

    [Test]
    public async Task ThenCorrectlyMapsLevyStatus_WhenNonLevy()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("NonLevy");

        var result = await fixture.Act();

        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsNull()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithLevyStatus(null);

        var result = await fixture.Act();

        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsEmpty()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithLevyStatus(string.Empty);

        var result = await fixture.Act();

        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsInvalid()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("InvalidValue");

        var result = await fixture.Act();

        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenCorrectlyMapsFilterModel()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithRequest(new ChangeEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "Test",
            SortField = "EmployerAccountLegalEntityName",
            ReverseSort = true,
            UseLearnerData = true
        });

        var result = await fixture.Act();

        result.SelectEmployerFilterModel.SearchTerm.Should().Be("Test");
        result.SelectEmployerFilterModel.CurrentlySortedByField.Should().Be("EmployerAccountLegalEntityName");
        result.SelectEmployerFilterModel.ReverseSort.Should().BeTrue();
        result.SelectEmployerFilterModel.UseLearnerData.Should().BeTrue();
    }

    [Test]
    public async Task ThenCorrectlyMapsEmployersListForAutocomplete()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture();
        fixture.WithMultipleEmployers();

        var result = await fixture.Act();

        result.SelectEmployerFilterModel.Employers.Should().Contain("Legal Entity 1");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Legal Entity 2");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Account 1");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Account 2");
    }

    [Test]
    public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
    {
        var fixture = new ChangeEmployerViewModelMapperFixture().WithNoMatchingEmployers();

        var result = await fixture.Act();

        ChangeEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
    }

    public class ChangeEmployerViewModelMapperFixture
    {
        private readonly ChangeEmployerViewModelMapper _sut;
        private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
        private ChangeEmployerRequest _request;
        private GetSelectEmployerResponse _apiResponse;

        public ChangeEmployerViewModelMapperFixture()
        {
            const long providerId = 123;
            var cacheKey = Guid.NewGuid();
            _request = new ChangeEmployerRequest { ProviderId = providerId, CacheKey = cacheKey };
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
                Employers = ["TestAccountLegalEntityName", "TestAccountName"]
            };

            _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            _sut = new ChangeEmployerViewModelMapper(_approvalsOuterApiClientMock.Object);
        }

        public ChangeEmployerViewModelMapperFixture WithRequest(ChangeEmployerRequest selectEmployerRequest)
        {
            _request = selectEmployerRequest;
            return this;
        }

        public ChangeEmployerViewModelMapperFixture WithLevyStatus(string apprenticeshipEmployerType)
        {
            _apiResponse.AccountProviderLegalEntities[0].ApprenticeshipEmployerType = apprenticeshipEmployerType;
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);
            return this;
        }

        public ChangeEmployerViewModelMapperFixture WithMultipleEmployers()
        {
            _apiResponse = new GetSelectEmployerResponse
            {
                AccountProviderLegalEntities =
                [
                    new()
                    {
                        AccountId = 1,
                        AccountLegalEntityPublicHashedId = "PUB1",
                        AccountLegalEntityName = "Legal Entity 1",
                        AccountPublicHashedId = "PUB1",
                        AccountName = "Account 1",
                        AccountLegalEntityId = 456,
                        AccountProviderId = 234,
                        AccountHashedId = "HASH1",
                        ApprenticeshipEmployerType = "Levy"
                    },

                    new()
                    {
                        AccountId = 2,
                        AccountLegalEntityPublicHashedId = "PUB2",
                        AccountLegalEntityName = "Legal Entity 2",
                        AccountPublicHashedId = "PUB2",
                        AccountName = "Account 2",
                        AccountLegalEntityId = 789,
                        AccountProviderId = 567,
                        AccountHashedId = "HASH2",
                        ApprenticeshipEmployerType = "NonLevy"
                    }
                ],
                Employers = ["Legal Entity 1", "Legal Entity 2", "Account 1", "Account 2"]
            };

            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            return this;
        }

        public async Task<ChangeEmployerViewModel> Act() => await _sut.Map(_request);

        public ChangeEmployerViewModelMapperFixture WithNoMatchingEmployers()
        {
            _apiResponse = new GetSelectEmployerResponse
            {
                AccountProviderLegalEntities = [],
                Employers = []
            };

            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            return this;
        }

        public void Verify_GetSelectEmployerWasCalled_Once()
        {
            _approvalsOuterApiClientMock.Verify(
                x => x.GetSelectEmployer(It.Is<GetSelectEmployerRequest>(r =>
                    r.GetUrl.Contains(_request.ProviderId.ToString()))), Times.Once);
        }

        public void Assert_ChangeEmployerViewModelCorrectlyMapped(ChangeEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(_apiResponse.AccountProviderLegalEntities.Count);
            result.ProviderId.Should().Be(_request.ProviderId);
            result.UseLearnerData.Should().Be(_request.UseLearnerData);
            result.CacheKey.Should().Be(_request.CacheKey);

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

        public static void Assert_ListOfEmployersIsEmpty(ChangeEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Should().BeEmpty();
        }
    }
}
