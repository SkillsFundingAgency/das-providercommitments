using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class WhenISelectStandardOption
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task Then_If_Option_Selected_Is_Minus_One_Then_Set_To_Empty_String()
        {
            _fixture
                .SetupCommitmentsApiToReturnADraftApprentice()
                .SetupUpdateRequestCourseOptionChooseLater()
                .SetupHasChosenToChooseOptionLater();
            
            await _fixture.PostToSelectOption();

            _fixture
                .VerifyApiUpdateWithStandardOptionSet("")
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public async Task Then_The_Option_Is_Saved_And_Redirected_To_Cohort_Details()
        {
            _fixture
                .SetupCommitmentsApiToReturnADraftApprentice()
                .SetupUpdateRequestCourseOption();
                
            await _fixture.PostToSelectOption();
            
            _fixture
                .VerifyApiUpdateWithStandardOptionSet()
                .VerifyRedirectedBackToCohortDetailsPage();
        }
    }
}