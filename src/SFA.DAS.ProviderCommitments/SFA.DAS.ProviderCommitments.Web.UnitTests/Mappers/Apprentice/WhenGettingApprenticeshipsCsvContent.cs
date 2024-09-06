using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture.NUnit3;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class WhenGettingApprenticeshipsCsvContent
    {
        [Test, MoqAutoData]
        public async Task Then_Passes_Filter_Args_To_Api(
            DownloadRequest csvRequest,
            [Frozen] Mock<IOuterApiService> mockApiService,
            DownloadApprenticesRequestMapper mapper)
        {
            // Arrange
            var apiRequestBody = new PostApprenticeshipsCSVRequest.Body
            {
                SearchTerm = csvRequest.SearchTerm,
                EmployerName = csvRequest.SelectedEmployer,
                CourseName = csvRequest.SelectedCourse,
                Status = csvRequest.SelectedStatus,
                StartDate = csvRequest.SelectedStartDate,
                EndDate = csvRequest.SelectedEndDate,
                Alert = csvRequest.SelectedAlert,
                ApprenticeConfirmationStatus = csvRequest.SelectedApprenticeConfirmation,
                DeliveryModel = csvRequest.SelectedDeliveryModel
            };

            var expectedRequest = new PostApprenticeshipsCSVRequest(csvRequest.ProviderId, apiRequestBody);

            // Act
            await mapper.Map(csvRequest);

            mockApiService.Verify(service => service.GetApprenticeshipsCSV(
               It.Is<PostApprenticeshipsCSVRequest>(actualRequest =>
                   actualRequest.ProviderId == expectedRequest.ProviderId &&
                   AreBodiesEqual((PostApprenticeshipsCSVRequest.Body)actualRequest.Data, apiRequestBody)
               )), Times.Once);          
        }

        [Test]
        public async Task ShouldMapValues()
        {
            //Arrange
            var fixture = new Fixture();
            var clientResponse = fixture.Create<PostApprenticeshipsCSVResponse>();
            var request = fixture.Create<DownloadRequest>();
            var client = new Mock<IOuterApiService>();
            var csvService = new Mock<ICreateCsvService>();
            var currentDateTime = new Mock<ICurrentDateTime>();
            var encodingService = new Mock<IEncodingService>();
            var expectedCsvContent = new byte[] { 1, 2, 3, 4 };
            var expectedMemoryStream = new MemoryStream(expectedCsvContent);
            currentDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 12, 30));
            var expectedFileName = $"{"Manageyourapprentices"}_{currentDateTime.Object.UtcNow:yyyyMMddhhmmss}.csv";

            var mapper = new DownloadApprenticesRequestMapper(client.Object, csvService.Object, currentDateTime.Object, encodingService.Object);

            client.Setup(x => x.GetApprenticeshipsCSV(It.Is<PostApprenticeshipsCSVRequest>(r =>
                    r.ProviderId.Equals(request.ProviderId)))).ReturnsAsync(clientResponse);

            csvService.Setup(x => x.GenerateCsvContent(It.IsAny<IEnumerable<ApprenticeshipDetailsCsvModel>>(), true))
                .Returns(expectedMemoryStream);

            //Act
            var content = await mapper.Map(request);

            Assert.Multiple(() =>
            {
                //Assert
                Assert.That(content.Name, Is.EqualTo(expectedFileName));
                Assert.That(content.Content, Is.EqualTo(expectedMemoryStream));
            });
        }

        private static bool AreBodiesEqual(PostApprenticeshipsCSVRequest.Body actual, PostApprenticeshipsCSVRequest.Body expected)
        {
            return actual.SearchTerm == expected.SearchTerm &&
                   actual.EmployerName == expected.EmployerName &&
                   actual.CourseName == expected.CourseName &&
                   actual.Status == expected.Status &&
                   actual.StartDate == expected.StartDate &&
                   actual.EndDate == expected.EndDate &&
                   actual.Alert == expected.Alert &&
                   actual.ApprenticeConfirmationStatus == expected.ApprenticeConfirmationStatus &&
                   actual.DeliveryModel == expected.DeliveryModel;
        }
    }
}
