using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test]
        public async Task ThenTheFileNameIsSetCorrectly()
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            var controller = new ManageApprenticesController(Mock.Of<IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                Mock.Of<IMapper<GetApprenticeshipsCsvContentRequest,byte[]>>());

            //Act
            var actual = await controller.Download(1);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actualFileResult.FileDownloadName);
        }

        [Test]
        public async Task ThenTheFileContentIsSetCorrectly()
        {
            //Arrange
            const uint providerId = 1;
            var expectedCsvContent = new byte[] {1, 2, 3, 4};
            var commitmentService = new Mock<ICommitmentsService>();
            var csvMapper = new Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>>();

            csvMapper.Setup(x =>
                    x.Map(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))))
                .ReturnsAsync(expectedCsvContent);

            var controller = new ManageApprenticesController(Mock.Of<IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                csvMapper.Object);

            //Act
            var actual = await controller.Download(providerId);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }

        [Test]
        public async Task ThenWillMapRequestToCsvContent()
        {
            //Arrange
            const uint providerId = 1;
            var commitmentService = new Mock<ICommitmentsService>();
            var csvMapper = new Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>>();

            var controller = new ManageApprenticesController(Mock.Of<IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                csvMapper.Object);

            //Act
            await controller.Download(providerId);

            //Assert
            csvMapper.Verify(x => x.Map(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))));
        }
    }
}
