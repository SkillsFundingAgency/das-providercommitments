using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingSelectAddDraftApprenticeshipJourney
    {
        [Test]
        public void ThenReturnsView()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var result = fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            var viewResult = fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }

        [Test]
        [TestCase(true, true, Description = "Toggle is enabled")]
        [TestCase(false, false, Description = "Toggle is disabled")]
        public void ThenFeatureToggleIsMapped(bool toggleValue, bool expectedValue)
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();

            if (toggleValue)
            {
                fixture.WithBulkUploadV2FeatureEnabled();
            }

            var viewResult = fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<SelectAddDraftApprenticeshipJourneyViewModel>();

            Assert.AreEqual(expectedValue, model.IsBulkUploadV2Enabled);
        }
    }

    public class WhenGettingSelectAddDraftApprenticeshipJourneyFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;
        private readonly Mock<IAuthorizationService> _featureToggleServiceMock;

        public WhenGettingSelectAddDraftApprenticeshipJourneyFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            _featureToggleServiceMock = new Mock<IAuthorizationService>();
            _featureToggleServiceMock.Setup(x => x.IsAuthorized(ProviderFeature.BulkUploadV2)).Returns(false);
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), _featureToggleServiceMock.Object, Mock.Of<IEncodingService>());
        }

        public IActionResult Act() => Sut.SelectAddDraftApprenticeshipJourney(_request);

        internal WhenGettingSelectAddDraftApprenticeshipJourneyFixture WithBulkUploadV2FeatureEnabled()
        {
            _featureToggleServiceMock.Setup(x => x.IsAuthorized(ProviderFeature.BulkUploadV2)).Returns(true);
            return this;
        }
    }
}