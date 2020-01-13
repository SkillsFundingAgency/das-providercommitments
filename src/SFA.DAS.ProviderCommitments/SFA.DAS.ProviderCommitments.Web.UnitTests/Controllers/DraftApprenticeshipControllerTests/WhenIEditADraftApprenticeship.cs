using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

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
        public async Task ShouldReturnEditDraftApprenticeshipView()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        public async Task ShouldSetProviderIdOnViewModel()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice().SetupProviderIdOnEditRequest(123);
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelHasProviderIdSet();
        }

        [Test]
        public async Task ShouldPassCohortIdAndDraftApprenticeshipIdToGetDraftApprenticeshipOnProviderCommitmentsService()
        {
            _fixture.SetupProviderCommitmentServiceToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyGetDraftApprenticeshipReceivesCorrectParameters();
        }

        [Test]
        public async Task AndWhenSavingTheDraftApprenticeIsSuccessful()
        {
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsDueToValidationItShouldThrowCommitmentsApiModelException()
        {

            _fixture.SetupUpdatingToThrowCommitmentsApiException();
            Assert.ThrowsAsync<CommitmentsApiModelException>(async () => await _fixture.PostToEditDraftApprenticeship());
        }
    }
}