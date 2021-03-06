﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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

            public UnapprovedControllerTestFixture()
            {
                var autoFixture = new Fixture();

                _mediator = new Mock<IMediator>();
                _mockModelMapper = new Mock<IModelMapper>();
                _linkGenerator = new Mock<ILinkGenerator>();

                _model = new AddDraftApprenticeshipViewModel
                {
                    ProviderId = autoFixture.Create<int>(),
                    EmployerAccountLegalEntityPublicHashedId = autoFixture.Create<string>(),
                    AccountLegalEntityId = autoFixture.Create<long>(),
                    ReservationId = autoFixture.Create<Guid>()
                };

                _createCohortRequest = new CreateCohortRequest();
                _mockModelMapper
                    .Setup(x => x.Map<CreateCohortRequest>(It.IsAny<AddDraftApprenticeshipViewModel>()))
                    .ReturnsAsync(_createCohortRequest);

                _createCohortResponse = new CreateCohortResponse
                {
                    CohortId = autoFixture.Create<long>(),
                    CohortReference = autoFixture.Create<string>()
                };

                _mediator.Setup(x => x.Send(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_createCohortResponse);

                _linkGeneratorRedirectUrl = autoFixture.Create<string>();
                _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink(It.IsAny<string>()))
                    .Returns(_linkGeneratorRedirectUrl)
                    .Callback((string value) => _linkGeneratorParameter = value);
                    
                
                _controller = new CohortController(_mediator.Object, _mockModelMapper.Object, _linkGenerator.Object, Mock.Of<ICommitmentsApiClient>());
            }

            public async Task<UnapprovedControllerTestFixture> PostDraftApprenticeshipViewModel()
            {
                _actionResult = await _controller.AddDraftApprenticeship(_model);
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
                var redirectResult = (RedirectResult)_actionResult;

                //1. verify that user was redirected to where the link gen said
                Assert.AreEqual(_linkGeneratorRedirectUrl, redirectResult.Url);

                //2. verify that we asked the link generator to gen a link to the correct page
                Assert.AreEqual($"{_model.ProviderId}/apprentices/{_createCohortResponse.CohortReference}/Details", _linkGeneratorParameter);

                return this;
            }
        }
    }
}
