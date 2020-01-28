using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
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
        public async Task ThenTheFileContentIsSetCorrectly(
            uint providerId,
            [Frozen] byte[] expectedCsvContent,
            [Frozen] Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>> csvMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            csvMapper.Setup(x =>
                    x.Map(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))))
                .ReturnsAsync(expectedCsvContent);

            //Act
            var actual = await controller.Download(providerId);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.IsNotNull(actualFileResult);
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }

        [Test, MoqAutoData]
        public async Task ThenWillMapRequestToCsvContent(
            uint providerId,
            [Frozen] byte[] expectedCsvContent,
            [Frozen] Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>> csvMapper,
            ManageApprenticesController controller)
        {
            //Act
            await controller.Download(providerId);

            //Assert
            csvMapper.Verify(x => x.Map(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))));
        }
    }
}
