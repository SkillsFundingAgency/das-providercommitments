using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapSelectEmployerRequestToViewModel
{
    [Test]
    public async Task ThenCallsGetSelectEmployerApi()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();

        // Act
        await fixture.Act();

        // Assert
        fixture.Verify_GetSelectEmployerWasCalled_Once();
    }

    [Test]
    public async Task ThenCorrectlyMapsApiResponseToViewModel()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();

        // Act
        var result = await fixture.Act();

        // Assert
        fixture.Assert_SelectEmployerViewModelCorrectlyMapped(result);
    }

    [Test]
    public async Task ThenCorrectlyMapsLevyStatus_WhenLevy()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("Levy");

        // Act
        var result = await fixture.Act();

        // Assert
        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.Levy);
    }

    [Test]
    public async Task ThenCorrectlyMapsLevyStatus_WhenNonLevy()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("NonLevy");

        // Act
        var result = await fixture.Act();

        // Assert
        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsNull()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithLevyStatus(null);

        // Act
        var result = await fixture.Act();

        // Assert
        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsEmpty()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithLevyStatus(string.Empty);

        // Act
        var result = await fixture.Act();

        // Assert
        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenDefaultsToNonLevy_WhenLevyStatusIsInvalid()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithLevyStatus("InvalidValue");

        // Act
        var result = await fixture.Act();

        // Assert
        result.AccountProviderLegalEntities[0].LevyStatus.Should().Be(ApprenticeshipEmployerType.NonLevy);
    }

    [Test]
    public async Task ThenCorrectlyMapsFilterModel()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithRequest(new SelectEmployerRequest
        {
            ProviderId = 123,
            SearchTerm = "Test",
            SortField = "EmployerAccountLegalEntityName",
            ReverseSort = true,
            UseLearnerData = true
        });

        // Act
        var result = await fixture.Act();

        // Assert
        result.SelectEmployerFilterModel.SearchTerm.Should().Be("Test");
        result.SelectEmployerFilterModel.CurrentlySortedByField.Should().Be("EmployerAccountLegalEntityName");
        result.SelectEmployerFilterModel.ReverseSort.Should().BeTrue();
        result.SelectEmployerFilterModel.UseLearnerData.Should().BeTrue();
        result.SelectEmployerFilterModel.ProviderId.Should().Be(123);
        result.SelectEmployerFilterModel.TotalEmployersFound.Should().Be(1);
        result.SelectEmployerFilterModel.PageNumber.Should().Be(1);
    }

    [Test]
    public async Task ThenCorrectlyMapsEmployersListForAutocomplete()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithMultipleEmployers();

        // Act
        var result = await fixture.Act();

        // Assert
        result.SelectEmployerFilterModel.Employers.Should().Contain("Legal Entity 1");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Legal Entity 2");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Account 1");
        result.SelectEmployerFilterModel.Employers.Should().Contain("Account 2");
    }

    [Test]
    public async Task ThenCorrectlyMapsEmptyApiResponseToViewModel()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture().WithNoMatchingEmployers();

        // Act
        var result = await fixture.Act();

        // Assert
        SelectEmployerViewModelMapperFixture.Assert_ListOfEmployersIsEmpty(result);
    }

    [Test]
    public async Task ThenPassesPageNumberAndPageSizeToApi_AndMapsTotalCountAndPageNumber()
    {
        // Arrange
        var fixture = new SelectEmployerViewModelMapperFixture();
        fixture.WithRequest(new SelectEmployerRequest
        {
            ProviderId = 456,
            PageNumber = 2
        });
        fixture.WithApiResponseTotalCount(250);

        // Act
        var result = await fixture.Act();

        // Assert
        result.SelectEmployerFilterModel.PageNumber.Should().Be(2);
        result.SelectEmployerFilterModel.TotalEmployersFound.Should().Be(250);
        result.SelectEmployerFilterModel.ShowPageLinks.Should().BeTrue();
        fixture.Verify_GetSelectEmployerWasCalled_WithPageNumberAndPageSize(2, Constants.SelectEmployer.NumberOfEmployersPerPage);
    }

    public class SelectEmployerViewModelMapperFixture
    {
        private readonly SelectEmployerViewModelMapper _sut;
        private readonly Mock<IApprovalsOuterApiClient> _approvalsOuterApiClientMock;
        private SelectEmployerRequest _request;
        private GetSelectEmployerResponse _apiResponse;

        public SelectEmployerViewModelMapperFixture()
        {
            const long providerId = 123;
            _request = new SelectEmployerRequest { ProviderId = providerId };
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
                Employers = ["TestAccountLegalEntityName", "TestAccountName"],
                TotalCount = 1
            };

            _approvalsOuterApiClientMock = new Mock<IApprovalsOuterApiClient>();
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            _sut = new SelectEmployerViewModelMapper(_approvalsOuterApiClientMock.Object);
        }

        public SelectEmployerViewModelMapperFixture WithRequest(SelectEmployerRequest selectEmployerRequest)
        {
            _request = selectEmployerRequest;
            return this;
        }

        public SelectEmployerViewModelMapperFixture WithLevyStatus(string apprenticeshipEmployerType)
        {
            _apiResponse.AccountProviderLegalEntities[0].ApprenticeshipEmployerType = apprenticeshipEmployerType;
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);
            return this;
        }

        public SelectEmployerViewModelMapperFixture WithMultipleEmployers()
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
                Employers = ["Legal Entity 1", "Legal Entity 2", "Account 1", "Account 2"],
                TotalCount = 2
            };

            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);

            return this;
        }

        public async Task<SelectEmployerViewModel> Act() => await _sut.Map(_request);

        public SelectEmployerViewModelMapperFixture WithNoMatchingEmployers()
        {
            _apiResponse = new GetSelectEmployerResponse
            {
                AccountProviderLegalEntities = [],
                Employers = [],
                TotalCount = 0
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

        public void Verify_GetSelectEmployerWasCalled_WithPageNumberAndPageSize(int pageNumber, int pageSize)
        {
            _approvalsOuterApiClientMock.Verify(
                x => x.GetSelectEmployer(It.Is<GetSelectEmployerRequest>(r =>
                    r.GetUrl.Contains($"pageNumber={pageNumber}") && r.GetUrl.Contains($"pageSize={pageSize}"))), Times.Once);
        }

        public SelectEmployerViewModelMapperFixture WithApiResponseTotalCount(int totalCount)
        {
            _apiResponse = new GetSelectEmployerResponse
            {
                AccountProviderLegalEntities = _apiResponse.AccountProviderLegalEntities,
                Employers = _apiResponse.Employers,
                TotalCount = totalCount
            };
            _approvalsOuterApiClientMock
                .Setup(x => x.GetSelectEmployer(It.IsAny<GetSelectEmployerRequest>()))
                .ReturnsAsync(_apiResponse);
            return this;
        }

        public void Assert_SelectEmployerViewModelCorrectlyMapped(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Count.Should().Be(_apiResponse.AccountProviderLegalEntities.Count);
            result.ProviderId.Should().Be(_request.ProviderId);
            result.UseLearnerData.Should().Be(_request.UseLearnerData);

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

        public static void Assert_ListOfEmployersIsEmpty(SelectEmployerViewModel result)
        {
            result.AccountProviderLegalEntities.Should().BeEmpty();
        }
    }
}
