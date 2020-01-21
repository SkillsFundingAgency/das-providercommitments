using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.Testing.AutoFixture;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipRequest;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsMapperTests
{
    public class WhenGettingApprenticeships
    {
        [Test, MoqAutoData]
        public async Task Should_Pass_Params_To_Api_Call(
            Requests.GetApprenticeshipsRequest webRequest,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(apiRequest => 
                        apiRequest.ProviderId == webRequest.ProviderId &&
                        apiRequest.PageNumber == webRequest.PageNumber &&
                        apiRequest.PageItemCount == webRequest.PageItemCount &&
                        //apiRequest.SearchTerm == webRequest.SearchTerm && todo: future story
                        apiRequest.EmployerName == webRequest.SelectedEmployer &&
                        apiRequest.CourseName == webRequest.SelectedCourse &&
                        apiRequest.Status == webRequest.SelectedStatus &&
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
                    It.IsAny<GetApprenticeshipsRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(
                webRequest.ProviderId, 
                It.IsAny<CancellationToken>()),
                Times.Once); 
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
                    It.IsAny<GetApprenticeshipsRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            await mapper.Map(webRequest);

            mockApiClient.Verify(client => client.GetApprenticeshipsFilterValues(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()),
                Times.Never); 
        }

        [Test, MoqAutoData]
        public async Task ShouldMapValues(
            Requests.GetApprenticeshipsRequest request,
            GetApprenticeshipsResponse apprenticeshipsResponse,
            GetApprenticeshipsFilterValuesResponse filtersResponse,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            apprenticeshipsResponse.TotalApprenticeships =
                ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 1;
            mockApiClient
                .Setup(x => x.GetApprenticeships(
                    It.IsAny<GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeshipsResponse);
            mockApiClient
                .Setup(client => client.GetApprenticeshipsFilterValues(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(filtersResponse);

            //Act
            var viewModel = await mapper.Map(request);

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(request.ProviderId, viewModel.ProviderId);
            Assert.AreEqual(apprenticeshipsResponse.Apprenticeships, viewModel.Apprenticeships);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeshipsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsFound);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeshipsWithAlertsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlertsFound);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeships, viewModel.FilterModel.TotalNumberOfApprenticeships);
            Assert.AreEqual(apprenticeshipsResponse.TotalApprenticeshipsWithAlerts, viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlerts);
            Assert.AreEqual(request.PageNumber, viewModel.FilterModel.PageNumber);
            Assert.AreEqual(filtersResponse.EmployerNames, viewModel.FilterModel.EmployerFilters);
            Assert.AreEqual(filtersResponse.CourseNames, viewModel.FilterModel.CourseFilters);
            Assert.AreEqual(filtersResponse.Statuses, viewModel.FilterModel.StatusFilters);
            Assert.AreEqual(filtersResponse.StartDates, viewModel.FilterModel.StartDateFilters);
            Assert.AreEqual(filtersResponse.EndDates, viewModel.FilterModel.EndDateFilters);
            Assert.AreEqual(request.SearchTerm, viewModel.FilterModel.SearchTerm);
            Assert.AreEqual(request.SelectedEmployer, viewModel.FilterModel.SelectedEmployer);
            Assert.AreEqual(request.SelectedCourse, viewModel.FilterModel.SelectedCourse);
            Assert.AreEqual(request.SelectedStatus, viewModel.FilterModel.SelectedStatus);
            Assert.AreEqual(request.SelectedStartDate, viewModel.FilterModel.SelectedStartDate);
            Assert.AreEqual(request.SelectedEndDate, viewModel.FilterModel.SelectedEndDate);
        }

        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(2, true)]
        public async Task ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNotNull(
            int numberOfApprenticeships, 
            bool expected,
            ApprenticeshipDetailsResponse approvedApprenticeship,
            Requests.GetApprenticeshipsRequest request,
            [Frozen]GetApprenticeshipsResponse clientResponse,
            Mock<ICommitmentsApiClient> client,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            var apprenticeships = new List<ApprenticeshipDetailsResponse>();

            for (var i = 0; i < numberOfApprenticeships; i++)
            {
                apprenticeships.Add(approvedApprenticeship);
            }

            clientResponse.Apprenticeships = apprenticeships;
           
            client.Setup(x => x.GetApprenticeships( It.Is<GetApprenticeshipsRequest>(r => 
                    r.ProviderId.Equals(request.ProviderId) &&
                    r.PageNumber.Equals(request.PageNumber) &&
                    r.PageItemCount.Equals(request.PageItemCount)),It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            //Act
            var viewModel = await mapper.Map(request);

            //Assert
            Assert.AreEqual(viewModel.AnyApprenticeships, expected);
        }
    }
}
