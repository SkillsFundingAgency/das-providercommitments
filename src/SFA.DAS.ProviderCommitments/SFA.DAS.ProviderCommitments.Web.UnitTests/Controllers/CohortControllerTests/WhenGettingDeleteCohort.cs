﻿using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderUrlHelper;
using AutoFixture;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingDeleteCohort
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new WhenGettingDeleteCohortFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingDeleteCohortFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<DeleteCohortViewModel>();
        }
    }

    public class WhenGettingDeleteCohortFixture
    {
        public CohortController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly DeleteCohortViewModel _viewModel;
        private readonly DeleteCohortRequest _request;

        public WhenGettingDeleteCohortFixture()
        {
            var fixture = new Fixture();
            _request = fixture.Create<DeleteCohortRequest>();
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<DeleteCohortViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<DeleteCohortViewModel>(_request))
                .ReturnsAsync(_viewModel);
            
            Sut = new CohortController(Mock.Of<IMediator>(),_modelMapperMock.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<DeleteCohortViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.Delete(_request);
    }
}
