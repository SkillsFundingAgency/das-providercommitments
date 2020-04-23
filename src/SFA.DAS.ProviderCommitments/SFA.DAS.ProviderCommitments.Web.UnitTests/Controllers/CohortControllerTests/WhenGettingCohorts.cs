using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingCohorts 
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var f = new WhenGettingCohortsFixture();

            await f.Sut.Cohorts(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<CohortsViewModel>(f.Request));
        }

        [Test]
        public async Task ThenReturnsCohortsViewModel()
        {
            var f = new WhenGettingCohortsFixture();

            var result = await f.Sut.Cohorts(f.Request) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(CohortsViewModel), result.Model.GetType());
        }

        [Test]
        public async Task ForReviewThenCallsModelMapper()
        {
            var f = new WhenGettingCohortsFixture();

            await f.Sut.Review2(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<ReviewViewModel2>(f.Request));
        }

        [Test]
        public async Task ForReviewThenReturnsReviewViewModel()
        {
            var f = new WhenGettingCohortsFixture();

            var result = await f.Sut.Review2(f.Request) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ReviewViewModel2), result.Model.GetType());
        }
    }

    public class WhenGettingCohortsFixture
    {
        public CohortController Sut { get; set; }
        public CohortsByProviderRequest Request { get; }

        public Mock<IModelMapper> ModelMapperMock { get; }
        public CohortsViewModel CohortsViewModel { get; }
        public ReviewViewModel2 ReviewViewModel2 { get; }

        public WhenGettingCohortsFixture()
        {
            Request = new CohortsByProviderRequest();
            ModelMapperMock = new Mock<IModelMapper>();
            CohortsViewModel = new CohortsViewModel();
            ReviewViewModel2 = new ReviewViewModel2();

            ModelMapperMock.Setup(x => x.Map<CohortsViewModel>(Request)).ReturnsAsync(CohortsViewModel);
            ModelMapperMock.Setup(x => x.Map<ReviewViewModel2>(Request)).ReturnsAsync(ReviewViewModel2);

            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            ModelMapperMock.Verify(x => x.Map<CohortsViewModel>(Request));
        }

        public async Task<IActionResult> Act() => await Sut.Cohorts(Request);
    }
}
