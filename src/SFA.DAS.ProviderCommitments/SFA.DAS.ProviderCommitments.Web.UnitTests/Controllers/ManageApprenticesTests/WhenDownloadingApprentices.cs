using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
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
            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            var controller = new ManageApprenticesController(Mock.Of<ICommitmentsService>(), Mock.Of<ICreateCsvService>());

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
            var createCsvService = new Mock<ICreateCsvService>();
            var controller = new ManageApprenticesController(Mock.Of<ICommitmentsService>(), createCsvService.Object);
            
            //Act
            await controller.Download(1);
            
            //Arrange
            createCsvService.Verify(x=>x.GenerateCsvContent(It.IsAny<List<ApprenticeshipDetailsCsvViewModel>>()));
        }
    }
}
