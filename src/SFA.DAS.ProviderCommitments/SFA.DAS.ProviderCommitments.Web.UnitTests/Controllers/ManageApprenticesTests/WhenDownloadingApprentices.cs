using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheApiIsCalledWithCorrectVariables(
            uint providerId,
            [Frozen]Mock<ICommitmentsService> mockCommitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            var sortField = "";
            var isReversed = false;

            //Act
            await controller.Download(providerId);

            //Assert
            mockCommitmentsService.Verify(x => x.GetApprenticeships(providerId, sortField, isReversed, true), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
            uint providerId,
            ManageApprenticesController controller)
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            
            //Act
            var actual = await controller.Download(providerId);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actualFileResult.FileDownloadName);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Result_Is_Mapped_To_The_Csv_View_Model(
            uint providerId,
            byte[] expectedCsvContent,
            [Frozen] Mock<ICreateCsvService> mockCsvService,
            ManageApprenticesController controller)
        {
            //Arrange
            mockCsvService
                .Setup(service => service.GenerateCsvContent(It.IsAny<IEnumerable<ApprenticeshipDetailsCsvViewModel>>()))
                .Returns(expectedCsvContent);
            //Act
            var actualFileResult = await controller.Download(providerId) as FileContentResult;
            
            //Assert
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }
    }
}
