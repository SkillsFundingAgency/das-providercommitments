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
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadCache
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadCacheFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_FileUploadCacheViewModel()
        {
            var fixture = new WhenGettingFileUploadCacheFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadCacheViewModel>();

            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingFileUploadCacheFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileUploadCacheRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadCacheViewModel _viewModel;

        public WhenGettingFileUploadCacheFixture()
        {
            var fixture = new AutoFixture.Fixture();
            

            _viewModel = fixture.Create<FileUploadCacheViewModel>();
            _request = new FileUploadCacheRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadCacheViewModel>(_request)).ReturnsAsync(_viewModel);
        
            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadCache(_request);
    }
}