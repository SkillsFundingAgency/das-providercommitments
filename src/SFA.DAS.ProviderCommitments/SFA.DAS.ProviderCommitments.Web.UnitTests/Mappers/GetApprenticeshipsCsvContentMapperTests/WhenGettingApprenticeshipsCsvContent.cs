using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsCsvContentMapperTests
{
    public class WhenGettingApprenticeshipsCsvContent
    {
        [Test, MoqAutoData]
        public async Task Then_Passes_Filter_Args_To_Api(
            DownloadRequest csvRequest,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            DownloadApprenticesRequestMapper mapper)
        {
            await mapper.Map(csvRequest);

            mockApiClient.Verify(client => client.GetApprenticeships(
                It.Is<GetApprenticeshipsRequest>(apiRequest =>
                    apiRequest.ProviderId == csvRequest.ProviderId &&
                    apiRequest.SearchTerm == csvRequest.SearchTerm && 
                    apiRequest.EmployerName == csvRequest.SelectedEmployer &&
                    apiRequest.CourseName == csvRequest.SelectedCourse &&
                    apiRequest.Status == csvRequest.SelectedStatus &&
                    apiRequest.StartDate == csvRequest.SelectedStartDate &&
                    apiRequest.EndDate == csvRequest.SelectedEndDate),
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ShouldMapValues()
        {
            //Arrange
            var fixture = new Fixture();
            var clientResponse = fixture.Create<GetApprenticeshipsResponse>();
            var request = fixture.Create<DownloadRequest>();
            var client = new Mock<ICommitmentsApiClient>();
            var csvService = new Mock<ICreateCsvService>();
            var currentDateTime = new Mock<ICurrentDateTime>();
            var expectedCsvContent = new byte[] {1, 2, 3, 4};
            currentDateTime.Setup(x => x.Now).Returns(new DateTime(2020, 12, 30));
            var expectedFileName = $"{"Manageyourapprentices"}_{currentDateTime.Object.Now:yyyyMMddhhmmss}.csv";

            var mapper = new DownloadApprenticesRequestMapper(client.Object, csvService.Object, currentDateTime.Object);

            client.Setup(x => x.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => 
                    r.ProviderId.Equals(request.ProviderId)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            csvService.Setup(x => x.GenerateCsvContent(It.IsAny<IEnumerable<ApprenticeshipDetailsCsvModel>>()))
                .Returns(expectedCsvContent);

            //Act
            var content = await mapper.Map(request);

            //Assert
            Assert.IsNotEmpty(content.Content);
            Assert.AreEqual(expectedCsvContent, content.Content);
            Assert.AreEqual(expectedFileName, content.Name);
        }
    }
}
