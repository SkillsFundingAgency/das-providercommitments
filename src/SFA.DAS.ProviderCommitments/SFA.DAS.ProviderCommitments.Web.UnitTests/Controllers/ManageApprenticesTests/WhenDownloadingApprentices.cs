using System;
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

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
            uint providerId,
            ManageApprenticesFilterModel filterModel,
            [Frozen] ICurrentDateTime mockDateTime,
            ManageApprenticesController controller)
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
            ManageApprenticesFilterModel filterModel,
            [Frozen] byte[] expectedCsvContent,
            [Frozen] Mock<IModelMapper> csvMapper,
            ManageApprenticesController controller)
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
            ManageApprenticesFilterModel filterModel,
            [Frozen] Mock<IModelMapper> csvMapper,
            ManageApprenticesController controller)
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
