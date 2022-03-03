using AutoFixture;
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
        public async Task Then_RedirectTo_ErrorPage_When_ProviderHasNoExistingCohort_And_HasNoCreateCohortPermissionAsync()
        {
            var fixture = new WhenGettingSelectAddDraftApprenticeshipJourneyFixture();
            fixture. ViewModel.HasExistingCohort = false;
            fixture.ViewModel.HasCreateCohortPermission = false;

            var result = await fixture.ActAsync();

            result.VerifyReturnsRedirectToActionResult().WithActionName("Error");
        }

    }

    public class WhenGettingSelectAddDraftApprenticeshipJourneyFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;
        private readonly Mock<IFeatureTogglesService<ProviderFeatureToggle>> _featureToggleServiceMock;
        private readonly Mock<IModelMapper> _modelMapperMock;
        public  SelectAddDraftApprenticeshipJourneyViewModel ViewModel { get; set; }


        public WhenGettingSelectAddDraftApprenticeshipJourneyFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            _featureToggleServiceMock = new Mock<IFeatureTogglesService<ProviderFeatureToggle>>();
            _featureToggleServiceMock.Setup(x => x.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix)).Returns(new ProviderFeatureToggle { IsEnabled = false });

            var fixture = new Fixture();
            ViewModel = fixture.Create<SelectAddDraftApprenticeshipJourneyViewModel>();
            ViewModel.ProviderId = ProviderId;
            ViewModel.HasExistingCohort = true;
            ViewModel.HasCreateCohortPermission = true;

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock
                .Setup(x => x.Map<SelectAddDraftApprenticeshipJourneyViewModel>(_request))
                .ReturnsAsync(ViewModel);

            Sut = new CohortController(Mock.Of<IMediator>(),
                _modelMapperMock.Object, 
                Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                _featureToggleServiceMock.Object,
                Mock.Of<IEncodingService>());
        }

        public async Task<IActionResult> ActAsync() => await Sut.SelectAddDraftApprenticeshipJourney(_request);

        internal WhenGettingSelectAddDraftApprenticeshipJourneyFixture WithBulkUploadV2FeatureEnabled()
        {
            _featureToggleServiceMock.Setup(x => x.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix)).Returns(new ProviderFeatureToggle { IsEnabled = true });
            return this;
        }
    }
}