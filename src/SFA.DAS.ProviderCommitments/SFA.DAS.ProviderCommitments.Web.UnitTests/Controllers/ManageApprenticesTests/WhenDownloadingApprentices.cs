using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task Then_The_File_Name_Is_Set_Correctly(
            uint providerId,
            ManageApprenticesController controller)
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            
            //Act
            var actual = await controller.Download(providerId) as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actual.FileDownloadName);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Result_Is_Mapped_To_The_Csv_View_Model(
            [Frozen] Mock<ICreateCsvService> mockCreateCsvService,
            ManageApprenticesController controller)
        {
            //Act
            await controller.Download(1);
            
            //Arrange
            mockCreateCsvService.Verify(x=>x.GenerateCsvContent(It.IsAny<List<ApprenticeshipDetailsCsvModel>>()));
        }
    }
}
