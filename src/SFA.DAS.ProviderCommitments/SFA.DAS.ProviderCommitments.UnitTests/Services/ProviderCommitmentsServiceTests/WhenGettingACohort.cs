using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services.ProviderCommitmentsServiceTests
{
    [TestFixture]
    public class WhenGettingACohort
    {
        private ProviderCommitmentsServiceTestFixtures _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new ProviderCommitmentsServiceTestFixtures();
        }

        [Test]
        public async Task ShouldMapValuesFromApiCallAndAddHashValues()
        {
            _fixture.SetupGetCohortDetailsReturnValue(_fixture.CohortApiDetail)
                .SetupHashingToEncodeInput();

           var result = await _fixture.Sut.GetCohortDetail(123);
            
            Assert.AreEqual(_fixture.CohortApiDetail.CohortId, result.CohortId);
            Assert.AreEqual(_fixture.CohortApiDetail.LegalEntityName, result.LegalEntityName);
            Assert.AreEqual($"CRX{_fixture.CohortApiDetail.CohortId}X", result.HashedCohortId);
        }

        [Test]
        public async Task ShouldCallClientApiWithCorrectParameter()
        {
            _fixture.SetupGetCohortDetailsReturnValue(_fixture.CohortApiDetail)
                .SetupHashingToEncodeInput();

            await _fixture.Sut.GetCohortDetail(123);

            _fixture.CommitmentsApiClientMock.Verify(x=>x.GetCohort(123, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}