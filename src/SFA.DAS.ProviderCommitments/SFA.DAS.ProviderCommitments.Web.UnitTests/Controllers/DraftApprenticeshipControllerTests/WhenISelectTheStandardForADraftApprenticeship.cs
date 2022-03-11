using NUnit.Framework;

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
        public void Then_If_The_Standard_Has_Not_Selected_Then_The_DraftApprentices_Is_Not_Called()
        {
            _fixture.SetUpNoStandardSelected();

            _fixture.PostToSelectStandard();

            _fixture.VerifyRedirectedBackToSelectStandardPage(); ;
        }

        [Test]
        public void Then_if_the_standard_can_be_delivered_flexibly_then_select_delivery_model()
        {
            _fixture.SetUpFlexibleStandardSelected();

            _fixture.PostToSelectStandard();

            _fixture.VerifyRedirectedToSelectDeliveryModelPage(); ;
        }
    }
}
