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
    public class WhenGettingChooseCohort
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var f = new WhenGettingChooseCohortFixture();

            await f.Sut.ChooseCohort(f.Request);

            f.ModelMapperMock.Verify(x => x.Map<ChooseCohortViewModel>(f.Request));
        }

        [Test]
        public async Task ThenReturnsChooseCohortsViewModel()
        {
            var f = new WhenGettingChooseCohortFixture();

            var result = await f.Sut.ChooseCohort(f.Request) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(ChooseCohortViewModel), result.Model.GetType());
        }
    }

    public class WhenGettingChooseCohortFixture
    {
        public CohortController Sut { get; set; }
        public ChooseCohortByProviderRequest Request { get; }
        public Mock<IModelMapper> ModelMapperMock { get; }
        public ChooseCohortViewModel ChooseCohortViewModel { get; }
        
        public WhenGettingChooseCohortFixture()
        {
            Request = new ChooseCohortByProviderRequest();
            ModelMapperMock = new Mock<IModelMapper>();
            ChooseCohortViewModel = new ChooseCohortViewModel();

            ModelMapperMock.Setup(x => x.Map<ChooseCohortViewModel>(Request)).ReturnsAsync(ChooseCohortViewModel);
            Sut = new CohortController(Mock.Of<IMediator>(), ModelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public async Task<IActionResult> Act() => await Sut.ChooseCohort(Request);
    }
}
