using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
            DownloadRequest request,
            [Frozen] ICurrentDateTime mockDateTime,
            ApprenticeController controller)
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{mockDateTime.Now:yyyyMMddhhmmss}.csv";

            //Act
            var actual = await controller.Download(request) as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actual.FileDownloadName);
        }

        [Test, MoqAutoData]
        public async Task ThenTheFileContentIsSetCorrectly(
            DownloadRequest request,
            [Frozen] byte[] expectedCsvContent,
            [Frozen] Mock<IModelMapper> csvMapper,
            ApprenticeController controller)
        {
            //Arrange
            csvMapper.Setup(x =>
                    x.Map<byte[]>(request))
                .ReturnsAsync(expectedCsvContent);

            //Act
            var actual = await controller.Download(request);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.IsNotNull(actualFileResult);
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }
    }
}
