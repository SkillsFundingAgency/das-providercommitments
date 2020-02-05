using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsCsvContentMapperTests
{
    public class WhenGettingApprenticeshipsCsvContent
    {
        [Test, MoqAutoData]
        public async Task Then_Passes_Filter_Args_To_Api(
            GetApprenticeshipsCsvContentRequest csvRequest,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetApprenticeshipsCsvContentRequestMapper mapper)
        {
            await mapper.Map(csvRequest);

            mockApiClient.Verify(client => client.GetApprenticeships(
                It.Is<GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.ProviderId == csvRequest.ProviderId &&
                    apiRequest.SearchTerm == csvRequest.FilterModel.SearchTerm && 
                    apiRequest.EmployerName == csvRequest.FilterModel.SelectedEmployer &&
                    apiRequest.CourseName == csvRequest.FilterModel.SelectedCourse &&
                    apiRequest.Status == csvRequest.FilterModel.SelectedStatus &&
                    apiRequest.StartDate == csvRequest.FilterModel.SelectedStartDate &&
                    apiRequest.EndDate == csvRequest.FilterModel.SelectedEndDate),
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ShouldMapValues()
        {
            //Arrange
            var fixture = new Fixture();
            var clientResponse = fixture.Create<GetApprenticeshipsResponse>();
            var request = fixture.Create<GetApprenticeshipsCsvContentRequest>();
            var client = new Mock<ICommitmentsApiClient>();
            var csvService = new Mock<ICreateCsvService>();
            var expectedCsvContent = new byte[] {1, 2, 3, 4};

            var mapper = new GetApprenticeshipsCsvContentRequestMapper(client.Object, csvService.Object);

            client.Setup(x => x.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => 
                    r.ProviderId.Equals(request.ProviderId)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            csvService.Setup(x => x.GenerateCsvContent(It.IsAny<IEnumerable<ApprenticeshipDetailsCsvModel>>()))
                .Returns(expectedCsvContent);

            //Act
            var content = await mapper.Map(request);

            //Assert
            Assert.IsNotEmpty(content);
            Assert.AreEqual(expectedCsvContent, content);
        }
    }
}
