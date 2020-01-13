using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsCsvContentMapperTests
{
    public class WhenGettingApprenticeshipsCsvContent
    {
        [Test]
        public async Task ShouldMapValues()
        {
            //Arrange
            var fixture = new Fixture();
            var clientResponse = fixture.Create<GetApprenticeshipsResponse>();
            var request = fixture.Create<GetApprenticeshipsCsvContentRequest>();
            var client = new Mock<ICommitmentsApiClient>();
            var csvService = new Mock<ICreateCsvService>();
            var expectedCsvContent = new byte[] {1, 2, 3, 4};

            var mapper = new GetApprenticeshipsCsvContentRequestMapper(client.Object, csvService.Object);

            client.Setup(x => x.GetApprenticeships(request.ProviderId, 0,0,null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            csvService.Setup(x => x.GenerateCsvContent(It.IsAny<IEnumerable<ApprenticeshipDetailsCsvViewModel>>()))
                .Returns(expectedCsvContent);

            //Act
            var content = await mapper.Map(request);

            //Assert
            Assert.IsNotEmpty(content);
            Assert.AreEqual(expectedCsvContent, content);
        }
    }
}
