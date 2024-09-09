using System;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

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

            try
            {
                await _fixture.PostToSelectStandard();
                throw new Exception("Should have thrown exception");
            }
            catch (CommitmentsApiModelException e)
            {
                e.Errors[0].Field.Should().Be("CourseCode");
                e.Errors[0].Message.Should().Be("You must select a training course");
            }
        }

        [Test]
        public async Task Then_if_the_standard_can_be_delivered_flexibly_then_select_delivery_model()
        {
            _fixture.SetUpFlexibleStandardSelected();

            await _fixture.PostToSelectStandard();

            _fixture.VerifyRedirectedToSelectDeliveryModelPage(); ;
        }
    }
}
