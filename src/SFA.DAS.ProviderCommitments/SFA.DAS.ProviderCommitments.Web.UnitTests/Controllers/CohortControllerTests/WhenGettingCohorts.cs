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
            var fixture = new WhenGettingCohortsFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingCohortsFixture();

            var result = await fixture.Act() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(CohortsViewModel), result.Model.GetType());
        }
    }

    public class WhenGettingCohortsFixture
    {
        public CohortController Sut { get; set; }
        
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly CohortsViewModel _viewModel;
        private readonly CohortsByProviderRequest _request;

        public WhenGettingCohortsFixture()
        {
            _request = new CohortsByProviderRequest();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = new CohortsViewModel();

            _modelMapperMock.Setup(x => x.Map<CohortsViewModel>(_request)).ReturnsAsync(_viewModel);

            Sut = new CohortController(Mock.Of<IMediator>(), _modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<CohortsViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.Cohorts(_request);
    }
}
