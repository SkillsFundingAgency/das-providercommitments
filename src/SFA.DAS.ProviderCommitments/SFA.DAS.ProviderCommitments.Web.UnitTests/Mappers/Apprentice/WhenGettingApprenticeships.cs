using System;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;
using ApiRequests = SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class WhenGettingApprenticeships
{
    [Test, MoqAutoData]
    public async Task Then_Defaults_To_Page_One(
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        var request = new IndexRequest();

        await mapper.Map(request);

        mockApiClient.Verify(client => client.GetApprenticeships(It.Is<ApiRequests.GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.PageNumber == 1 &&
                    apiRequest.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Defaults_To_Page_One_If_Less_Than_One(
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        var request = new IndexRequest { PageNumber = 0 };

        await mapper.Map(request);

        mockApiClient.Verify(client => client.GetApprenticeships(It.Is<ApiRequests.GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.PageNumber == 1 &&
                    apiRequest.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Should_Pass_Params_To_Api_Call(
        IndexRequest webRequest,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        await mapper.Map(webRequest);

        mockApiClient.Verify(client => client.GetApprenticeships(It.Is<ApiRequests.GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.ProviderId == webRequest.ProviderId &&
                    apiRequest.PageNumber == webRequest.PageNumber &&
                    apiRequest.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage &&
                    apiRequest.SearchTerm == webRequest.SearchTerm &&
                    apiRequest.EmployerName == webRequest.SelectedEmployer &&
                    apiRequest.CourseName == webRequest.SelectedCourse &&
                    apiRequest.ApprenticeConfirmationStatus == webRequest.SelectedApprenticeConfirmation &&
                    apiRequest.DeliveryModel == webRequest.SelectedDeliveryModel &&
                    apiRequest.Status == webRequest.SelectedStatus &&
                    apiRequest.StartDate == webRequest.SelectedStartDate &&
                    apiRequest.EndDate == webRequest.SelectedEndDate &&
                    apiRequest.IsOnFlexiPaymentPilot == webRequest.SelectedPilotStatus),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Gets_Filter_Values_From_Api(
        IndexRequest webRequest,
        GetApprenticeshipsResponse clientResponse,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;
        mockApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<ApiRequests.GetApprenticeshipsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientResponse);


        await mapper.Map(webRequest);

        mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(
            It.Is<ApiRequests.GetApprenticeshipFiltersRequest>(
                r => r.ProviderId.Equals(webRequest.ProviderId)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task And_TotalApprentices_Less_Than_NumberOfApprenticesRequiredForSearch_Then_Not_Get_Filter_Values_From_Api(
        IndexRequest webRequest,
        GetApprenticeshipsResponse clientResponse,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.TotalApprenticeships = Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch - 1;

        mockApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<ApiRequests.GetApprenticeshipsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientResponse);

        await mapper.Map(webRequest);

        mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(
                It.IsAny<ApiRequests.GetApprenticeshipFiltersRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test, MoqAutoData]
    public async Task ShouldMapApiValues(
        IndexRequest request,
        GetApprenticeshipsResponse apprenticeshipsResponse,
        GetApprenticeshipsFilterValuesResponse filtersResponse,
        ApprenticeshipDetailsViewModel expectedViewModel,
        [Frozen] Mock<IModelMapper> modelMapper,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        //Arrange
        apprenticeshipsResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;

        mockApiClient
            .Setup(x => x.GetApprenticeships(
                It.IsAny<ApiRequests.GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apprenticeshipsResponse);

        mockApiClient
            .Setup(client => client.GetApprenticeshipsFilterValues(
                It.IsAny<ApiRequests.GetApprenticeshipFiltersRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(filtersResponse);

        modelMapper
            .Setup(x => x.Map<ApprenticeshipDetailsViewModel>(It.IsAny<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()))
            .ReturnsAsync(expectedViewModel);

        //Act
        var viewModel = await mapper.Map(request);

        //Assert
        using (new AssertionScope())
        {
            viewModel.Apprenticeships.Should().AllBeEquivalentTo(expectedViewModel);
            viewModel.Should().NotBeNull();
            viewModel.ProviderId.Should().Be(request.ProviderId);
            viewModel.FilterModel.ProviderId.Should().Be(request.ProviderId);
            viewModel.FilterModel.TotalNumberOfApprenticeshipsFound.Should().Be(apprenticeshipsResponse.TotalApprenticeshipsFound);
            viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlertsFound.Should().Be(apprenticeshipsResponse.TotalApprenticeshipsWithAlertsFound);
            viewModel.FilterModel.TotalNumberOfApprenticeships.Should().Be(apprenticeshipsResponse.TotalApprenticeships);
            viewModel.FilterModel.PageNumber.Should().Be(apprenticeshipsResponse.PageNumber);
            viewModel.FilterModel.ReverseSort.Should().Be(request.ReverseSort);
            viewModel.FilterModel.SortField.Should().Be(request.SortField);
            viewModel.FilterModel.EmployerFilters.Should().BeEquivalentTo(filtersResponse.EmployerNames);
            viewModel.FilterModel.CourseFilters.Should().BeEquivalentTo(filtersResponse.CourseNames);
            viewModel.FilterModel.StartDateFilters.Should().BeEquivalentTo(filtersResponse.StartDates);
            viewModel.FilterModel.EndDateFilters.Should().BeEquivalentTo(filtersResponse.EndDates);
            viewModel.FilterModel.SearchTerm.Should().Be(request.SearchTerm);
            viewModel.FilterModel.SelectedEmployer.Should().Be(request.SelectedEmployer);
            viewModel.FilterModel.SelectedCourse.Should().Be(request.SelectedCourse);
            viewModel.FilterModel.SelectedStatus.Should().Be(request.SelectedStatus);
            viewModel.FilterModel.SelectedStartDate.Should().Be(request.SelectedStartDate);
            viewModel.FilterModel.SelectedEndDate.Should().Be(request.SelectedEndDate);
            viewModel.FilterModel.SelectedApprenticeConfirmation.Should().Be(request.SelectedApprenticeConfirmation);
            viewModel.FilterModel.SelectedDeliveryModel.Should().Be(request.SelectedDeliveryModel);
            viewModel.FilterModel.SelectedPilotStatus.Should().Be(request.SelectedPilotStatus);
        }
    }

    [Test, MoqAutoData]
    public async Task ShouldMapStatusValues(
        IndexRequest request,
        GetApprenticeshipsResponse apprenticeshipsResponse,
        GetApprenticeshipsFilterValuesResponse filtersResponse,
        ApprenticeshipDetailsViewModel expectedViewModel,
        [Frozen] Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>>
            detailsViewModelMapper,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        //Arrange
        apprenticeshipsResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;

        mockApiClient
            .Setup(x => x.GetApprenticeships(
                It.IsAny<ApiRequests.GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apprenticeshipsResponse);

        mockApiClient
            .Setup(client => client.GetApprenticeshipsFilterValues(
                It.IsAny<ApiRequests.GetApprenticeshipFiltersRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(filtersResponse);

        detailsViewModelMapper
            .Setup(x => x.Map(It.IsAny<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()))
            .ReturnsAsync(expectedViewModel);

        //Act
        var viewModel = await mapper.Map(request);

        using (new AssertionScope())
        {
            viewModel.FilterModel.StatusFilters.Should().Contain(ApprenticeshipStatus.Live);
            viewModel.FilterModel.StatusFilters.Should().Contain(ApprenticeshipStatus.Paused);
            viewModel.FilterModel.StatusFilters.Should().Contain(ApprenticeshipStatus.Stopped);
            viewModel.FilterModel.StatusFilters.Should().Contain(ApprenticeshipStatus.WaitingToStart);
            viewModel.FilterModel.StatusFilters.Should().Contain(ApprenticeshipStatus.Completed);
            viewModel.FilterModel.StatusFilters.Should().NotContain(ApprenticeshipStatus.Unknown);
        }
    }

    [Test, MoqAutoData]
    public async Task ThenWillSetPageNumberToLastOneIfRequestPageNumberIsTooHigh(
        IndexRequest webRequest,
        GetApprenticeshipsResponse clientResponse,
        [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.PageNumber = (int)Math.Ceiling((double)clientResponse.TotalApprenticeshipsFound / Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage);
        webRequest.PageNumber = clientResponse.PageNumber + 10;

        clientResponse.TotalApprenticeships = Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch - 1;

        mockApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<ApiRequests.GetApprenticeshipsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientResponse);

        var result = await mapper.Map(webRequest);

        using (new AssertionScope())
        {
            result.FilterModel.PageLinks.Count(x => x.IsCurrent.HasValue && x.IsCurrent.Value).Should().Be(1);
            result.FilterModel.PageLinks.Last().IsCurrent.Should().BeTrue();
        }
    }
}