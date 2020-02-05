using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
            uint providerId,
            ApprenticesFilterModel filterModel,
            [Frozen] ICurrentDateTime mockDateTime,
            ApprenticeController controller)
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{mockDateTime.Now:yyyyMMddhhmmss}.csv";

            //Act
            var actual = await controller.Download(providerId, filterModel) as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actual.FileDownloadName);
        }

        [Test, MoqAutoData]
        public async Task ThenTheFileContentIsSetCorrectly(
            uint providerId,
            ApprenticesFilterModel filterModel,
            [Frozen] byte[] expectedCsvContent,
            [Frozen] Mock<IModelMapper> csvMapper,
            ApprenticeController controller)
        {
            //Arrange
            csvMapper.Setup(x =>
                    x.Map<byte[]>(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))))
                .ReturnsAsync(expectedCsvContent);

            //Act
            var actual = await controller.Download(providerId, filterModel);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.IsNotNull(actualFileResult);
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }

        [Test, MoqAutoData]
        public async Task ThenWillMapRequestToCsvContent(
            uint providerId,
            ApprenticesFilterModel filterModel,
            [Frozen] Mock<IModelMapper> csvMapper,
            ApprenticeController controller)
        {
            //Act
            await controller.Download(providerId, filterModel);

            //Assert
            csvMapper.Verify(x => x.Map<byte[]>(
                It.Is<GetApprenticeshipsCsvContentRequest>(request => 
                    request.ProviderId.Equals(providerId) &&
                    request.FilterModel.Equals(filterModel))));
        }
    }
}
