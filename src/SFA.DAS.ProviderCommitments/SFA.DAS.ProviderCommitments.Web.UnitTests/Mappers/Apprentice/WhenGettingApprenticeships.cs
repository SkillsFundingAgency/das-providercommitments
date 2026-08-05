using System;
using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class WhenGettingApprenticeships
{
    [Test, MoqAutoData]
    public async Task Then_Defaults_To_Page_One(
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        long providerId,
        IndexViewModelMapper mapper)
    {
        var request = new IndexRequest { ProviderId = providerId };

        await mapper.Map(request);

        mockapprovalsOuterApiClient.Verify(client => client.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.PageNumber == 1 && apiRequest.ProviderId == providerId &&
                    apiRequest.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage)),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Defaults_To_Page_One_If_Less_Than_One(
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        long providerId,
        IndexViewModelMapper mapper)
    {
        var request = new IndexRequest { PageNumber = 0, ProviderId = providerId };

        await mapper.Map(request);

        mockapprovalsOuterApiClient.Verify(client => client.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.PageNumber == 1 && apiRequest.ProviderId == providerId &&
                    apiRequest.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage)),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Should_Pass_Params_To_Api_Call(
        IndexRequest webRequest,
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        await mapper.Map(webRequest);

        mockapprovalsOuterApiClient.Verify(client => client.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(apiRequest =>
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
                    apiRequest.EndDate == webRequest.SelectedEndDate)),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Gets_Filter_Values_From_Api(
        IndexRequest webRequest,
        GetApprenticeshipsResponse clientResponse,
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;
        mockapprovalsOuterApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<GetApprenticeshipsRequest>()))
            .ReturnsAsync(clientResponse);

        await mapper.Map(webRequest);
    }

    [Test, MoqAutoData]
    public async Task And_TotalApprentices_Less_Than_NumberOfApprenticesRequiredForSearch_Then_Not_Get_Filter_Values_From_Api(
        IndexRequest webRequest,
        GetApprenticeshipsResponse clientResponse,
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.TotalApprenticeships = Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch - 1;

        mockapprovalsOuterApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<GetApprenticeshipsRequest>()))
            .ReturnsAsync(clientResponse);

        await mapper.Map(webRequest);
    }

    [Test, MoqAutoData]
    public async Task ShouldMapApiValues(
        IndexRequest request,
        GetApprenticeshipsResponse apprenticeshipsResponse,
        ApprenticeshipDetailsViewModel expectedViewModel,
        [Frozen] Mock<IModelMapper> modelMapper,
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        //Arrange
        apprenticeshipsResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;

        mockapprovalsOuterApiClient
            .Setup(x => x.GetApprenticeships(
                It.IsAny<GetApprenticeshipsRequest>()))
            .ReturnsAsync(apprenticeshipsResponse);

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
            viewModel.FilterModel.EmployerFilters.Should().BeEquivalentTo(apprenticeshipsResponse.ApprenticeshipFiltersValue.EmployerNames);
            viewModel.FilterModel.CourseFilters.Should().BeEquivalentTo(apprenticeshipsResponse.ApprenticeshipFiltersValue.CourseNames);
            viewModel.FilterModel.StartDateFilters.Should().BeEquivalentTo(apprenticeshipsResponse.ApprenticeshipFiltersValue.StartDates);
            viewModel.FilterModel.EndDateFilters.Should().BeEquivalentTo(apprenticeshipsResponse.ApprenticeshipFiltersValue.EndDates);
            viewModel.FilterModel.SearchTerm.Should().Be(request.SearchTerm);
            viewModel.FilterModel.SelectedEmployer.Should().Be(request.SelectedEmployer);
            viewModel.FilterModel.SelectedCourse.Should().Be(request.SelectedCourse);
            viewModel.FilterModel.SelectedStatus.Should().Be(request.SelectedStatus);
            viewModel.FilterModel.SelectedStartDate.Should().Be(request.SelectedStartDate);
            viewModel.FilterModel.SelectedEndDate.Should().Be(request.SelectedEndDate);
            viewModel.FilterModel.SelectedApprenticeConfirmation.Should().Be(request.SelectedApprenticeConfirmation);
            viewModel.FilterModel.SelectedDeliveryModel.Should().Be(request.SelectedDeliveryModel);
        }
    }

    [Test, MoqAutoData]
    public async Task ShouldMapStatusValues(
        IndexRequest request,
        GetApprenticeshipsResponse apprenticeshipsResponse,
        GetApprenticeshipsFiltersResponse filtersResponse,
        ApprenticeshipDetailsViewModel expectedViewModel,
        [Frozen] Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>>
            detailsViewModelMapper,
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        //Arrange
        apprenticeshipsResponse.TotalApprenticeships =
            Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch + 1;

        mockapprovalsOuterApiClient
            .Setup(x => x.GetApprenticeships(
                It.IsAny<GetApprenticeshipsRequest>()))
            .ReturnsAsync(apprenticeshipsResponse);

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
        [Frozen] Mock<IApprovalsOuterApiClient> mockapprovalsOuterApiClient,
        IndexViewModelMapper mapper)
    {
        clientResponse.PageNumber = (int)Math.Ceiling((double)clientResponse.TotalApprenticeshipsFound / Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage);
        webRequest.PageNumber = clientResponse.PageNumber + 10;

        clientResponse.TotalApprenticeships = Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch - 1;

        mockapprovalsOuterApiClient
            .Setup(client => client.GetApprenticeships(
                It.IsAny<GetApprenticeshipsRequest>()))
            .ReturnsAsync(clientResponse);

        var result = await mapper.Map(webRequest);

        using (new AssertionScope())
        {
            result.FilterModel.PageLinks.Count(x => x.IsCurrent.HasValue && x.IsCurrent.Value).Should().Be(1);
            result.FilterModel.PageLinks.Last().IsCurrent.Should().BeTrue();
        }
    }
}