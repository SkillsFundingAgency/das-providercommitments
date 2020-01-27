using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    public class WhenDownloadingApprentices
    {
        [Test, AutoData]
        public async Task ThenTheFileNameIsSetCorrectly(
            long providerId,
            ManageApprenticesFilterModel filterModel)
        {
            //Arrange
            var expected = $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv";
            var controller = new ManageApprenticesController(Mock.Of<IMapper<Requests.GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                Mock.Of<IMapper<GetApprenticeshipsCsvContentRequest,byte[]>>());

            //Act
            var actual = await controller.Download(providerId, filterModel);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expected, actualFileResult.FileDownloadName);
        }

        [Test, AutoData]
        public async Task ThenTheFileContentIsSetCorrectly(
            long providerId,
            ManageApprenticesFilterModel filterModel)
        {
            //Arrange
            var expectedCsvContent = new byte[] {1, 2, 3, 4};
            var commitmentService = new Mock<ICommitmentsService>();
            var csvMapper = new Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>>();

            csvMapper.Setup(x =>
                    x.Map(It.Is<GetApprenticeshipsCsvContentRequest>(request => request.ProviderId.Equals(providerId))))
                .ReturnsAsync(expectedCsvContent);

            var controller = new ManageApprenticesController(Mock.Of<IMapper<Requests.GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                csvMapper.Object);

            //Act
            var actual = await controller.Download(providerId, filterModel);

            var actualFileResult = actual as FileContentResult;

            //Assert
            Assert.AreEqual(expectedCsvContent, actualFileResult.FileContents);
        }

        [Test, AutoData]
        public async Task ThenWillMapRequestToCsvContent(
            long providerId,
            ManageApprenticesFilterModel filterModel)
        {
            //Arrange
            var commitmentService = new Mock<ICommitmentsService>();
            var csvMapper = new Mock<IMapper<GetApprenticeshipsCsvContentRequest, byte[]>>();

            var controller = new ManageApprenticesController(Mock.Of<IMapper<Requests.GetApprenticeshipsRequest,ManageApprenticesViewModel>>(), 
                csvMapper.Object);

            //Act
            await controller.Download(providerId, filterModel);

            //Assert
            csvMapper.Verify(x => x.Map(
                It.Is<GetApprenticeshipsCsvContentRequest>(request => 
                    request.ProviderId.Equals(providerId) &&
                    request.FilterModel.Equals(filterModel))));
        }
    }
}
