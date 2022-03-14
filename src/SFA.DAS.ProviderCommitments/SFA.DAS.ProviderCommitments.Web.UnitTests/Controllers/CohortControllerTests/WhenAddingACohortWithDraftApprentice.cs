using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenAddingACohortWithDraftApprentice
    {
        [Test]
        public async Task ThenRedirectedToAddApprenticeship()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = await fixture.Act() as RedirectToActionResult;

            result.ActionName.Should().Be("AddDraftApprenticeship");
        }

        [Test]
        public async Task AndDeliveryModelToggleIsOn_ThenRedirectedToSelectCourse()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetDeliveryModelToggleOn();

            var result = await fixture.Act() as RedirectToActionResult;

            result.ActionName.Should().Be("SelectCourse");
        }

        [Test]
        public async Task AndOnAddApprenticeshipPage_ThenReturnsView()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = await fixture.ActOnAddApprenticeship();

            result.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();
        }

        [Test]
        public async Task AndOnAddApprenticeshipPage_ThenMapperIsCalled()
        {
            var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

            var result = await fixture.ActOnAddApprenticeship();

            fixture.VerifyMapperWasCalled();
        }
    }

    public class WhenAddingACohortWithDraftApprenticeFixture
    {
        public CohortController Sut { get; set; }
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly Mock<IFeatureTogglesService<ProviderFeatureToggle>> _providerFeatureToggle;
        private readonly AddDraftApprenticeshipViewModel _viewModel;
        private readonly CreateCohortWithDraftApprenticeshipRequest _request;

        public WhenAddingACohortWithDraftApprenticeFixture()
        {
            _request = new CreateCohortWithDraftApprenticeshipRequest();
            _viewModel = new AddDraftApprenticeshipViewModel();
            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(_viewModel);
            _providerFeatureToggle = new Mock<IFeatureTogglesService<ProviderFeatureToggle>>();
            _providerFeatureToggle.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new ProviderFeatureToggle());

            Sut = new CohortController(
                Mock.Of<IMediator>(),
                _modelMapper.Object, 
                Mock.Of<ILinkGenerator>(), 
                Mock.Of<ICommitmentsApiClient>(),
                _providerFeatureToggle.Object,
                Mock.Of<IEncodingService>());
        }

        public WhenAddingACohortWithDraftApprenticeFixture SetDeliveryModelToggleOn()
        {
            _providerFeatureToggle.Setup(x => x.GetFeatureToggle(It.Is<string>(p=>p == "DeliveryModel"))).Returns(new ProviderFeatureToggle { IsEnabled = true });
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.AddNewDraftApprenticeship(_request);
        public async Task<IActionResult> ActOnAddApprenticeship() => await Sut.AddDraftApprenticeship(_request);
    }
}