﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenRequestingConfirmEditApprenticeshipTests
    {
        private WhenRequestingConfirmEditApprenticeshipFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenRequestingConfirmEditApprenticeshipFixture();
        }

        [Test]
        public async Task VerifyViewModelMapperIsCalled()
        {
            await _fixture.ConfirmEditApprenticeship();
            _fixture.VerifyViewModelMapperIsCalled();
        }

        [Test]
        public async Task VerifyViewIsReturned()
        {
            var result = await _fixture.ConfirmEditApprenticeship();
            WhenRequestingConfirmEditApprenticeshipFixture.VerifyViewResultIsReturned(result);
        }
    }

    public class WhenRequestingConfirmEditApprenticeshipFixture : ApprenticeControllerTestFixtureBase
    {
        public WhenRequestingConfirmEditApprenticeshipFixture() : base()
        {
            Controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
        }

        public async Task<IActionResult> ConfirmEditApprenticeship()
        {
            return await Controller.ConfirmEditApprenticeship();
        }
        
        internal void VerifyViewModelMapperIsCalled()
        {
            MockMapper.Verify(x => x.Map<ConfirmEditApprenticeshipViewModel>(It.IsAny<EditApprenticeshipRequestViewModel>()), Times.Once());
        }

        internal static void VerifyViewResultIsReturned(IActionResult result)
        {
            result.Should().BeAssignableTo<ViewResult>();
        }
    }
}
