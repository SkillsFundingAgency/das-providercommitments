using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIEditADraftApprenticeship
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        [Ignore("Test Fixture problem")]
        public async Task ShouldReturnEditDraftApprenticeshipView()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        [Ignore("Test Fixture problem")]
        public async Task ShouldPassCohortIdAndDraftApprenticeshipIdToGetDraftApprenticeshipOnProviderCommitmentsService()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyGetDraftApprenticeshipReceivesCorrectParameters();
        }


        [Test]
        public async Task IfCalledWithAnInvalidRequestShouldGetBadResponseReturned()
        {
            _fixture.SetupModelStateToBeInvalid();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyWeGetABadRequestResponse();
        }
    }
}