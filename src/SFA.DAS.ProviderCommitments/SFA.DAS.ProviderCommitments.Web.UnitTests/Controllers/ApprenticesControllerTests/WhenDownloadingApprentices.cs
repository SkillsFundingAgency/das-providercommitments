using System.IO;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenDownloadingApprentices
    {

        [Test, MoqAutoData]
        public async Task ThenTheFileContentIsSetCorrectly(
            DownloadRequest request,
            string expectedFileName,
            [Frozen] Mock<HttpContext> httpContext,
            [Frozen] Mock<IModelMapper> csvMapper,
            [Greedy] ApprenticeController controller)
        {
            //Arrange
            var expectedCsvContent = new DownloadViewModel
            {
                Name = expectedFileName,
                Content = new MemoryStream()
            };
            csvMapper.Setup(x =>
                    x.Map<DownloadViewModel>(request))
                .ReturnsAsync(expectedCsvContent);
     
            httpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            //Act
            var actual = await controller.Download(request);

            var actualFileResult = actual as FileResult;

            //Assert
            Assert.IsNotNull(actualFileResult);
            Assert.AreEqual(expectedCsvContent.Name, actualFileResult.FileDownloadName);
            Assert.AreEqual(actualFileResult.ContentType, actualFileResult.ContentType);
        }
    }

}
