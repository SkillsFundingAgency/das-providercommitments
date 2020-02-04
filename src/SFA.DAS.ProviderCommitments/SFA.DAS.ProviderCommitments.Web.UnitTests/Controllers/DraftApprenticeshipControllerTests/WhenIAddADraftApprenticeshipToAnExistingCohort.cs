using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIAddADraftApprenticeshipToAnExistingCohort
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task IfCalledViaReservationsItShouldReturnAddDraftApprenticeshipViewWithCohortAndWithAReservationId()
        {
            await _fixture.AddDraftApprenticeshipWithReservation();
            _fixture.VerifyAddViewHasCohortWithAReservationId()
                .VerifyCohortDetailsWasCalledWithCorrectId()
                .VerifyGetCoursesWasCalled();
        }

        [Test]
        public async Task AndWhenSavingTheApprenticeToCohortIsSuccessful()
        {
            await _fixture.PostToAddDraftApprenticeship();
            _fixture.VerifyMappingToApiTypeIsCalled()
                .VerifyApiAddMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task AndWhenSavingFailsBecauseOfModelValidationItShouldThrowCommitmentApiModelException()
        {
            _fixture.SetupAddingToThrowCommitmentsApiException();
            Assert.ThrowsAsync<CommitmentsApiModelException>(async ()=>  await _fixture.PostToAddDraftApprenticeship());
        }
    }
}