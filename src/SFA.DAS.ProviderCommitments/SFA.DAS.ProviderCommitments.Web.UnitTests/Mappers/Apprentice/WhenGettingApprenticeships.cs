using System;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;
using ApiRequests = SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
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
            var request = new IndexRequest {PageNumber = 0};

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
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.ProviderId, Is.EqualTo(request.ProviderId));
            viewModel.Apprenticeships.Should().AllBeEquivalentTo(expectedViewModel);
            Assert.That(viewModel.FilterModel.TotalNumberOfApprenticeshipsFound, Is.EqualTo(apprenticeshipsResponse.TotalApprenticeshipsFound));
            Assert.That(viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlertsFound, Is.EqualTo(apprenticeshipsResponse.TotalApprenticeshipsWithAlertsFound));
            Assert.That(viewModel.FilterModel.TotalNumberOfApprenticeships, Is.EqualTo(apprenticeshipsResponse.TotalApprenticeships));
            Assert.That(viewModel.FilterModel.PageNumber, Is.EqualTo(apprenticeshipsResponse.PageNumber));
            Assert.That(viewModel.FilterModel.ReverseSort, Is.EqualTo(request.ReverseSort));
            Assert.That(viewModel.FilterModel.SortField, Is.EqualTo(request.SortField));
            Assert.That(viewModel.FilterModel.EmployerFilters, Is.EqualTo(filtersResponse.EmployerNames));
            Assert.That(viewModel.FilterModel.CourseFilters, Is.EqualTo(filtersResponse.CourseNames));
            Assert.That(viewModel.FilterModel.StartDateFilters, Is.EqualTo(filtersResponse.StartDates));
            Assert.That(viewModel.FilterModel.EndDateFilters, Is.EqualTo(filtersResponse.EndDates));
            Assert.That(viewModel.FilterModel.SearchTerm, Is.EqualTo(request.SearchTerm));
            Assert.That(viewModel.FilterModel.SelectedEmployer, Is.EqualTo(request.SelectedEmployer));
            Assert.That(viewModel.FilterModel.SelectedCourse, Is.EqualTo(request.SelectedCourse));
            Assert.That(viewModel.FilterModel.SelectedStatus, Is.EqualTo(request.SelectedStatus));
            Assert.That(viewModel.FilterModel.SelectedStartDate, Is.EqualTo(request.SelectedStartDate));
            Assert.That(viewModel.FilterModel.SelectedEndDate, Is.EqualTo(request.SelectedEndDate));
            Assert.That(viewModel.FilterModel.SelectedApprenticeConfirmation, Is.EqualTo(request.SelectedApprenticeConfirmation));
            Assert.That(viewModel.FilterModel.SelectedDeliveryModel, Is.EqualTo(request.SelectedDeliveryModel));
            Assert.That(viewModel.FilterModel.SelectedPilotStatus, Is.EqualTo(request.SelectedPilotStatus));
        }

        [Test, MoqAutoData]
        public async Task ShouldMapStatusValues(
            IndexRequest request,
            GetApprenticeshipsResponse apprenticeshipsResponse,
            GetApprenticeshipsFilterValuesResponse filtersResponse,
            ApprenticeshipDetailsViewModel expectedViewModel,
            [Frozen]
            Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>>
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

            Assert.That(viewModel.FilterModel.StatusFilters, Does.Contain(ApprenticeshipStatus.Live));
            Assert.That(viewModel.FilterModel.StatusFilters, Does.Contain(ApprenticeshipStatus.Paused));
            Assert.That(viewModel.FilterModel.StatusFilters, Does.Contain(ApprenticeshipStatus.Stopped));
            Assert.That(viewModel.FilterModel.StatusFilters, Does.Contain(ApprenticeshipStatus.WaitingToStart));
            Assert.That(viewModel.FilterModel.StatusFilters, Does.Contain(ApprenticeshipStatus.Completed));
            Assert.That(viewModel.FilterModel.StatusFilters, Does.Not.Contain(ApprenticeshipStatus.Unknown));
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

            var result= await mapper.Map(webRequest);

            Assert.That(result.FilterModel.PageLinks.Count(x => x.IsCurrent.HasValue && x.IsCurrent.Value), Is.EqualTo(1));
            Assert.That(result.FilterModel.PageLinks.Last().IsCurrent, Is.True);
        }
    }
}
