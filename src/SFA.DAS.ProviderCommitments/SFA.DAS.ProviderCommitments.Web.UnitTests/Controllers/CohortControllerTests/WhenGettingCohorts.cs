using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingCohorts 
    {
        [Test]
        public async Task ForReviewThenCallsModelMapper()
        {
            var f = new WhenGettingCohortsFixture();

            await f.Sut.Review(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<ReviewViewModel>(f.Request));
        }

        [Test]
        public async Task ForReviewThenReturnsReviewViewModel()
        {
            var f = new WhenGettingCohortsFixture();

            var result = await f.Sut.Review(f.Request) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ReviewViewModel), result.Model.GetType());
        }

        [Test]
        public async Task ForDraftThenCallsModelMapper()
        {
            var f = new WhenGettingCohortsFixture();

            await f.Sut.Draft(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<DraftViewModel>(f.Request));
        }

        [Test]
        public async Task ForDraftThenReturnsDraftViewModel()
        {
            var f = new WhenGettingCohortsFixture();

            var result = await f.Sut.Draft(f.Request) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(DraftViewModel), result.Model.GetType());
        }

        [Test]
        public async Task ForWithTransferSenderThenCallsModelMapper()
        {
            var f = new WhenGettingCohortsFixture();

            await f.Sut.WithTransferSender(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<WithTransferSenderViewModel>(f.Request));
        }

        [Test]
        public async Task ForWithTransferSenderThenReturnsWithTransferSenderViewModel()
        {
            var f = new WhenGettingCohortsFixture();

            var result = await f.Sut.WithTransferSender(f.Request);

            result.VerifyReturnsViewModel().WithModel<WithTransferSenderViewModel>();
        }
    }

    public class WhenGettingCohortsFixture
    {
        public CohortController Sut { get; set; }
        public CohortsByProviderRequest Request { get; }
        public Mock<IModelMapper> ModelMapperMock { get; }
        public CohortsViewModel CohortsViewModel { get; }
        public ReviewViewModel ReviewViewModel { get; }
        public DraftViewModel DraftViewModel { get; }
        public WithTransferSenderViewModel WithTransferSenderViewModel { get; }

        public WhenGettingCohortsFixture()
        {
            Request = new CohortsByProviderRequest();
            ModelMapperMock = new Mock<IModelMapper>();
            CohortsViewModel = new CohortsViewModel();
            ReviewViewModel = new ReviewViewModel();
            DraftViewModel = new DraftViewModel();
            WithTransferSenderViewModel = new WithTransferSenderViewModel();

            ModelMapperMock.Setup(x => x.Map<CohortsViewModel>(Request)).ReturnsAsync(CohortsViewModel);
            ModelMapperMock.Setup(x => x.Map<ReviewViewModel>(Request)).ReturnsAsync(ReviewViewModel);
            ModelMapperMock.Setup(x => x.Map<DraftViewModel>(Request)).ReturnsAsync(DraftViewModel);
            ModelMapperMock.Setup(x => x.Map<WithTransferSenderViewModel>(Request)).ReturnsAsync(WithTransferSenderViewModel);

            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public async Task<IActionResult> Act() => await Sut.Review(Request);
    }
}
