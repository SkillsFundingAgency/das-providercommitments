using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenGettingCancelChangeOfCircumstance
    {
        private CancelChangeOfCircumstanceRequest _request;

        private WhenGettingCancelChangeOfCircumstanceFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();

            _request = autoFixture.Create<CancelChangeOfCircumstanceRequest>();

            _fixture = new WhenGettingCancelChangeOfCircumstanceFixture();
        }

        [Test]
        public void VerifyTempDataRemoved()
        {
            _fixture.CancelChangeOfCircumstance(_request);

            _fixture.VerifyTempDataRemoved();
        }

        [Test]
        public void VerifyRedirectToDetails()
        {
            var result = _fixture.CancelChangeOfCircumstance(_request);

            _fixture.VerifyRedirectToDetails(result as RedirectToActionResult);
        }
    }

    public class WhenGettingCancelChangeOfCircumstanceFixture : ApprenticeControllerTestFixtureBase
    {

        private Mock<ITempDataDictionary> _mockTempData;

        public WhenGettingCancelChangeOfCircumstanceFixture() : base()
        {

            _mockTempData = new Mock<ITempDataDictionary>();

            _controller.TempData = _mockTempData.Object;
        }

        public IActionResult CancelChangeOfCircumstance(CancelChangeOfCircumstanceRequest request)
        {
            return _controller.CancelChangeOfCircumstance(request);
        }

        public void VerifyTempDataRemoved()
        {
            _mockTempData.Verify(d => d.Remove("EditApprenticeshipRequestViewModel"), Times.Once);
        }

        public void VerifyRedirectToDetails(RedirectToActionResult redirectResult)
        {
            redirectResult.ActionName.Should().Be("Details");
        }
    }
}
