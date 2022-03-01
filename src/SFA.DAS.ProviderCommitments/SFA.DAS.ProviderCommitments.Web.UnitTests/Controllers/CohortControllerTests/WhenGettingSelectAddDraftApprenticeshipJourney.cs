using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingSelectAddDraftApprenticeshipJourney
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var result = await fixture.ActAsync();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var viewResult = await fixture.ActAsync();

            var model =  viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }

        [Test]
        [TestCase(true, true, Description = "Toggle is enabled")]
        [TestCase(false, false, Description = "Toggle is disabled")]
        public async Task ThenFeatureToggleIsMappedAsync(bool toggleValue, bool expectedValue)
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            if (toggleValue)
            {
                fixture.WithBulkUploadV2FeatureEnabled();
            }

            var viewResult = await fixture.ActAsync();

            var model = viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.AreEqual(expectedValue, model.IsBulkUploadV2Enabled);
        }
    }

    public class WhenGettingSelectAddDraftApprenticeshipJourneyFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;
        private readonly Mock<IFeatureTogglesService<ProviderFeatureToggle>> _featureToggleServiceMock;

        public WhenGettingSelectAddDraftApprenticeshipJourneyFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            _featureToggleServiceMock = new Mock<IFeatureTogglesService<ProviderFeatureToggle>>();
            _featureToggleServiceMock.Setup(x => x.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix)).Returns(new ProviderFeatureToggle { IsEnabled = false });
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), _featureToggleServiceMock.Object, Mock.Of<IEncodingService>());
        }

        public async Task<IActionResult> ActAsync() => await Sut.SelectAddDraftApprenticeshipJourney(_request);

        internal WhenGettingSelectAddDraftApprenticeshipJourneyFixture WithBulkUploadV2FeatureEnabled()
        {
            _featureToggleServiceMock.Setup(x => x.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix)).Returns(new ProviderFeatureToggle { IsEnabled = true });
            return this;
        }
    }
}