using NUnit.Framework;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class WhenISelectTheStandardForADraftApprenticeship
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task Then_If_The_Standard_Has_Not_Selected_Then_The_DraftApprentices_Is_Not_Called()
        {
            _fixture.SetUpNoStandardSelected();

            await _fixture.PostToSelectStandard();

            _fixture.VerifyRedirectedBackToSelectStandardPage(); ;
        }
    }
}
