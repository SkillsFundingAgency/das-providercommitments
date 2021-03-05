using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingACohortWithDraftApprentice
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new WhenGettingACohortWithDraftApprenticeFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingACohortWithDraftApprenticeFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();
        }
    }

    public class WhenGettingACohortWithDraftApprenticeFixture
    {
        public CohortController Sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly AddDraftApprenticeshipViewModel _viewModel;
        private readonly CreateCohortWithDraftApprenticeshipRequest _request;

        public WhenGettingACohortWithDraftApprenticeFixture()
        {
            _request = new CreateCohortWithDraftApprenticeshipRequest();
            _viewModel = new AddDraftApprenticeshipViewModel();
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(_viewModel);
            
            Sut = new CohortController(Mock.Of<IMediator>(),_modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.AddDraftApprenticeship(_request);
    }
}