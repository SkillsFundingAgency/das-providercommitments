﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;
using ApiRequests = SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsMapperTests
{
    public class WhenGettingApprenticeships
    {
        [Test, MoqAutoData]
        public async Task Should_Pass_Params_To_Api_Call(
            Requests.GetApprenticeshipsRequest webRequest,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            [Frozen] Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>> detailsViewModelMapper,
            ApprenticeshipDetailsViewModel expectedViewModel, 
            GetApprenticeshipsRequestMapper mapper)
        {
            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeships(It.Is<ApiRequests.GetApprenticeshipsRequest>(apiRequest => 
                        apiRequest.ProviderId == webRequest.ProviderId &&
                        apiRequest.PageNumber == webRequest.PageNumber &&
                        apiRequest.PageItemCount == webRequest.PageItemCount &&
                        //apiRequest.SearchTerm == webRequest.SearchTerm && todo: future story
                        apiRequest.EmployerName == webRequest.SelectedEmployer &&
                        apiRequest.CourseName == webRequest.SelectedCourse &&
                        apiRequest.Status == webRequest.SelectedStatus.ToString() &&
                        apiRequest.StartDate == webRequest.SelectedStartDate &&
                        apiRequest.EndDate == webRequest.SelectedEndDate),
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_Filter_Values_From_Api(
            Requests.GetApprenticeshipsRequest webRequest,
            GetApprenticeshipsResponse clientResponse,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            clientResponse.TotalApprenticeships =
                ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 1;
            mockApiClient
                .Setup(client => client.GetApprenticeships(
                    It.IsAny<ApiRequests.GetApprenticeshipsRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

           
            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(webRequest.ProviderId,It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_TotalApprentices_Less_Than_NumberOfApprenticesRequiredForSearch_Then_Not_Get_Filter_Values_From_Api(
            Requests.GetApprenticeshipsRequest webRequest,
            GetApprenticeshipsResponse clientResponse,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            clientResponse.TotalApprenticeships = ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch - 1;
            
            mockApiClient
                .Setup(client => client.GetApprenticeships(
                    It.IsAny<ApiRequests.GetApprenticeshipsRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()),
                Times.Never); 
        }

        [Test, MoqAutoData]
        public async Task ShouldMapApiValues(
            Requests.GetApprenticeshipsRequest request,
            GetApprenticeshipsResponse apprenticeshipsResponse,
            GetApprenticeshipsFilterValuesResponse filtersResponse,
            ApprenticeshipDetailsViewModel expectedViewModel,
            [Frozen] Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>> detailsViewModelMapper,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            apprenticeshipsResponse.TotalApprenticeships =
                ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 1;
            
            mockApiClient
                .Setup(x => x.GetApprenticeships(
                    It.IsAny<ApiRequests.GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeshipsResponse);
            
            mockApiClient
                .Setup(client => client.GetApprenticeshipsFilterValues(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(filtersResponse);
            
            detailsViewModelMapper
                .Setup(x => x.Map(It.IsAny<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()))
                .ReturnsAsync(expectedViewModel);

            //Act
            var viewModel = await mapper.Map(request);

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(request.ProviderId, viewModel.ProviderId);
            viewModel.Apprenticeships.Should().AllBeEquivalentTo(expectedViewModel);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeshipsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsFound);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeshipsWithAlertsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlertsFound);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeships, viewModel.FilterModel.TotalNumberOfApprenticeships);
            Assert.AreEqual(request.PageNumber, viewModel.FilterModel.PageNumber);
            Assert.AreEqual(request.ReverseSort,viewModel.FilterModel.ReverseSort);
            Assert.AreEqual(request.SortField, viewModel.FilterModel.SortField);
            Assert.AreEqual(filtersResponse.EmployerNames, viewModel.FilterModel.EmployerFilters);
            Assert.AreEqual(filtersResponse.CourseNames, viewModel.FilterModel.CourseFilters);
            Assert.AreEqual(filtersResponse.StartDates, viewModel.FilterModel.StartDateFilters);
            Assert.AreEqual(filtersResponse.EndDates, viewModel.FilterModel.EndDateFilters);
            Assert.AreEqual(request.SearchTerm, viewModel.FilterModel.SearchTerm);
            Assert.AreEqual(request.SelectedEmployer, viewModel.FilterModel.SelectedEmployer);
            Assert.AreEqual(request.SelectedCourse, viewModel.FilterModel.SelectedCourse);
            Assert.AreEqual(request.SelectedStatus, viewModel.FilterModel.SelectedStatus);
            Assert.AreEqual(request.SelectedStartDate, viewModel.FilterModel.SelectedStartDate);
            Assert.AreEqual(request.SelectedEndDate, viewModel.FilterModel.SelectedEndDate);
        }

        [Test, MoqAutoData]
        public async Task ShouldMapStatusValues(
            Requests.GetApprenticeshipsRequest request,
            GetApprenticeshipsResponse apprenticeshipsResponse,
            GetApprenticeshipsFilterValuesResponse filtersResponse,
            ApprenticeshipDetailsViewModel expectedViewModel,
            [Frozen]
            Mock<IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel>>
                detailsViewModelMapper,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            apprenticeshipsResponse.TotalApprenticeships =
                ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 1;

            mockApiClient
                .Setup(x => x.GetApprenticeships(
                    It.IsAny<ApiRequests.GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeshipsResponse);

            mockApiClient
                .Setup(client => client.GetApprenticeshipsFilterValues(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(filtersResponse);

            detailsViewModelMapper
                .Setup(x => x.Map(It.IsAny<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()))
                .ReturnsAsync(expectedViewModel);

            //Act
            var viewModel = await mapper.Map(request);
            
            Assert.IsTrue(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.Live));
            Assert.IsTrue(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.Paused));
            Assert.IsTrue(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.Stopped));
            Assert.IsTrue(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.WaitingToStart));

            Assert.IsFalse(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.None));
            Assert.IsFalse(viewModel.FilterModel.StatusFilters.Contains(ApprenticeshipStatus.Completed));
        }
    }
}
