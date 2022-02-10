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
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileToDiscard
    {
        [Test]
        public void ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingFileToDiscardFixture();

            //Act
            var result =  fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenReturnsView_With_FileDiscardViewModel()
        {
            //Arrange
            var fixture = new WhenGettingFileToDiscardFixture();

            //Act
            var viewResult = fixture.Act();

            //Assert
            var model = viewResult.VerifyReturnsViewModel().WithModel<FileDiscardViewModel>();
            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingFileToDiscardFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileDiscardRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileDiscardViewModel _viewModel;

        public WhenGettingFileToDiscardFixture()
        {
            var fixture = new AutoFixture.Fixture();

            _viewModel = fixture.Create<FileDiscardViewModel>();
            _request = new FileDiscardRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileDiscardViewModel>(_request)).ReturnsAsync(_viewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public IActionResult Act() => _sut.FileDiscard(_request);
    }
}
