using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test]
        public async Task ThenTheFileNameIsSetCorrectly()
        {
            //Arrange
            var commitmentService = new Mock<ICommitmentsService>();

            commitmentService.Setup(x => x.GetApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(new GetApprenticeshipsFilteredResult
                {
                    Apprenticeships = new List<ApprenticeshipDetails>(),
                    TotalNumberOfApprenticeshipsFound = 0
                });

            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            var controller = new ManageApprenticesController(commitmentService.Object, Mock.Of<ICreateCsvService>());

            //Act
            var actual = await controller.Download(1);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actualFileResult.FileDownloadName);
        }

        [Test]
        public async Task Then_The_Result_Is_Mapped_To_The_Csv_View_Model()
        {
            //Arrange
            var commitmentService = new Mock<ICommitmentsService>();

            commitmentService.Setup(x => x.GetApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(new GetApprenticeshipsFilteredResult()
                {
                    Apprenticeships = new List<ApprenticeshipDetails>(),
                    TotalNumberOfApprenticeshipsFound = 0
                });

            var createCsvService = new Mock<ICreateCsvService>();
            var controller = new ManageApprenticesController(commitmentService.Object, createCsvService.Object);
            
            //Act
            await controller.Download(1);
            
            //Arrange
            createCsvService.Verify(x=>x.GenerateCsvContent(It.IsAny<List<ApprenticeshipDetailsCsvViewModel>>()));
        }
    }
}
