using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.ActionResults;
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
            [Frozen] Mock<IModelMapper> csvMapper,
            ApprenticeController controller)
        {
            //Arrange
            var expectedCsvContent = new DownloadViewModel
            {
                Name = expectedFileName
            };
            csvMapper.Setup(x =>
                    x.Map<DownloadViewModel>(request))
                .ReturnsAsync(expectedCsvContent);

            //Act
            var actual = await controller.Download(request);

            var actualFileResult = actual as FileCallbackResult;

            //Assert
            Assert.IsNotNull(actualFileResult);
            Assert.AreEqual(expectedCsvContent.Name, actualFileResult.FileDownloadName);
            Assert.AreEqual(expectedCsvContent.ContentType, actualFileResult.ContentType);
        }
    }
}
