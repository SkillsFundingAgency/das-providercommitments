using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostDraftApprenticeshipViewModel
    {
        private UnapprovedControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new UnapprovedControllerTestFixture();
        }

        [Test]
        public async Task ThenACohortIsCreated()
        {
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyCohortCreated();
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToTheViewCohortPage()
        {
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirection();
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToStandardOptionsIfAvailable()
        {
            _fixture.SetupHasOptions();
            await _fixture.PostDraftApprenticeshipViewModel();
            _fixture.VerifyUserRedirectSelectOption();
        }

        private class UnapprovedControllerTestFixture
        {
            private readonly CohortController _controller;
            private readonly Mock<IMediator> _mediator;
            private readonly Mock<IModelMapper> _mockModelMapper;
            private readonly Mock<ILinkGenerator> _linkGenerator;
            private readonly AddDraftApprenticeshipViewModel _model;
            private readonly CreateCohortRequest _createCohortRequest;
            private readonly CreateCohortResponse _createCohortResponse;
            private IActionResult _actionResult;
            private readonly string _linkGeneratorRedirectUrl;
            private string _linkGeneratorParameter;
            private Fixture _autoFixture;
            private readonly Mock<IEncodingService> _encodingService;
            private readonly string _draftApprenticeshipHashedId;

            public UnapprovedControllerTestFixture()
            {
                _autoFixture = new Fixture();

                _draftApprenticeshipHashedId = _autoFixture.Create<string>();
                _mediator = new Mock<IMediator>();
                _mockModelMapper = new Mock<IModelMapper>();
                _linkGenerator = new Mock<ILinkGenerator>();
                _encodingService = new Mock<IEncodingService>();

                _model = new AddDraftApprenticeshipViewModel
                {
                    ProviderId = _autoFixture.Create<int>(),
                    EmployerAccountLegalEntityPublicHashedId = _autoFixture.Create<string>(),
                    AccountLegalEntityId = _autoFixture.Create<long>(),
                    ReservationId = _autoFixture.Create<Guid>()
                };

                _createCohortRequest = new CreateCohortRequest();
                _mockModelMapper
                    .Setup(x => x.Map<CreateCohortRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                    .ReturnsAsync(_createCohortRequest);

                _createCohortResponse = new CreateCohortResponse
                {
                    CohortId = _autoFixture.Create<long>(),
                    CohortReference = _autoFixture.Create<string>(),
                    DraftApprenticeshipId = null
                };

                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_createCohortResponse);

                _linkGeneratorRedirectUrl = _autoFixture.Create<string>();
                _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorRedirectUrl)
                    .Callback((string value) => _linkGeneratorParameter = value);
                    
                
                _controller = new CohortController(_mediator.Object, _mockModelMapper.Object, _linkGenerator.Object, Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), _encodingService.Object);
            }

            public async Task<UnapprovedControllerTestFixture> PostDraftApprenticeshipViewModel()
            {
                _actionResult = await _controller.SaveDraftApprenticeship(_model);
                return this;
            }

            public UnapprovedControllerTestFixture SetupHasOptions()
            {
                var draftApprenticeshipId = _autoFixture.Create<long>();

                _encodingService.Setup(x => x.Encode(draftApprenticeshipId, EncodingType.ApprenticeshipId))
                    .Returns(_draftApprenticeshipHashedId);
                
                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new CreateCohortResponse
                    {
                        CohortId = _autoFixture.Create<long>(),
                        CohortReference = _autoFixture.Create<string>(),
                        DraftApprenticeshipId = draftApprenticeshipId
                    });
                return this;
            }

            public UnapprovedControllerTestFixture VerifyCohortCreated()
            {
                //1. Verify that the viewmodel submitted was mapped
                _mockModelMapper.Verify(x => x.Map<CreateCohortRequest>(It.Is<AddDraftApprenticeshipViewModel>(m => m == _model)),Times.Once);
                //2. Verify that the mapper result (request) was sent
                _mediator.Verify(x => x.Send(It.Is<CreateCohortRequest>(r => r == _createCohortRequest), It.IsAny<CancellationToken>()), Times.Once);
                return this;
            }

            public UnapprovedControllerTestFixture VerifyUserRedirection()
            {
                _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("Details");
                return this;
            }

            public UnapprovedControllerTestFixture VerifyUserRedirectSelectOption()
            {
                _actionResult.VerifyReturnsRedirectToActionResult().WithActionName("SelectOptions");
                var result = _actionResult as RedirectToActionResult;
                Assert.IsNotNull(result);
                result.RouteValues["DraftApprenticeshipHashedId"].Should().Be(_draftApprenticeshipHashedId);
                return this;
            }
        }
    }
}
