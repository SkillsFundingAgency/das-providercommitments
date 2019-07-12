using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.Commitments.Shared.UnitTests
{
    [TestFixture]
    public class CommitmentsServiceTests
    {
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new CommitmentsServiceTestFixtures();

            fixtures.CreateCommitmentsService();

            Assert.Pass("Did not get an exception");
        }

        [Test]
        public void Constructor_NullApiClient_ShouldShouldThrowNullArgumentException()
        {
            var fixtures = new CommitmentsServiceTestFixtures();

            var ex = Assert.Throws<ArgumentNullException>(() =>fixtures.CreateCommitmentsServiceWithNullClient());
            Assert.IsTrue(ex.Message.Contains("client", StringComparison.InvariantCultureIgnoreCase), ex.Message);
        }

        [Test]
        public void Constructor_NullEncodingService_ShouldShouldThrowNullArgumentException()
        {
            var fixtures = new CommitmentsServiceTestFixtures();

            var ex  = Assert.Throws<ArgumentNullException>(() => fixtures.CreateCommitmentsServiceWithNullEncodingService());
            Assert.IsTrue(ex.Message.Contains("encodingservice", StringComparison.InvariantCultureIgnoreCase), ex.Message);
        }
    }

    public class CommitmentsServiceTestFixtures
    {
        public CommitmentsServiceTestFixtures()
        {
            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();    
            EncodingServiceMock = new Mock<IEncodingService>();
        }

        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock { get; }
        public ICommitmentsApiClient CommitmentsApiClient => CommitmentsApiClientMock.Object;

        public Mock<IEncodingService> EncodingServiceMock { get; }
        public IEncodingService EncodingService => EncodingServiceMock.Object;

        public CommitmentsService CreateCommitmentsService()
        {
            return new CommitmentsService(CommitmentsApiClient, EncodingService);
        }

        public CommitmentsService CreateCommitmentsServiceWithNullClient()
        {
            return new CommitmentsService(null, EncodingService);
        }

        public CommitmentsService CreateCommitmentsServiceWithNullEncodingService()
        {
            return new CommitmentsService(CommitmentsApiClient, null);
        }

        public CommitmentsServiceTestFixtures WithDraftApprenticeship(long cohortId, long draftApprenticeshipId)
        {
            CommitmentsApiClientMock
                .Setup(cac =>
                    cac.GetDraftApprenticeship(cohortId, draftApprenticeshipId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetDraftApprenticeshipResponse
                {
                    Id = draftApprenticeshipId
                });

            return this;
        }
    }
}
