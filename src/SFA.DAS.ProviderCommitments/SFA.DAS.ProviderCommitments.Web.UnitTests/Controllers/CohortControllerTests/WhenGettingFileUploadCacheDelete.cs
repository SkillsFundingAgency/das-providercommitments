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
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadCacheDelete
    {
        [TestCase(null)]
        [TestCase(FileUploadCacheDeleteRedirect.UploadAgain)]
        public void Then_Redirects_To_FileUploadStart(FileUploadCacheDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadCacheDeleteFixture();

            var result =  fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadStart");
        }


        [TestCase(FileUploadCacheDeleteRedirect.Home)]
        public void Then_Redirects_To_Home(FileUploadCacheDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadCacheDeleteFixture();

            var result = fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirect().WithUrl("pasurl/account");
        }

        [Test]
        public void Then_Cache_Is_Cleared()
        {
            var fixture = new WhenGettingFileUploadCacheDeleteFixture();

            fixture.Act();

            fixture.VerifyCacheIsCleared();
        }
    }

    public class WhenGettingFileUploadCacheDeleteFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileUploadCacheDeleteRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadCacheViewModel _viewModel;
        private readonly Mock<ICacheService> _cacheService;
        private readonly Mock<ILinkGenerator> _linkGenerator;

        public WhenGettingFileUploadCacheDeleteFixture()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Create<FileUploadCacheViewModel>();
            _request = new FileUploadCacheDeleteRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadCacheViewModel>(_request)).ReturnsAsync(_viewModel);
            _cacheService = new Mock<ICacheService>();

            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink("/account")).Returns("pasurl/account");
        
            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, _linkGenerator.Object, Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public IActionResult Act() => _sut.FileUploadCacheDelete(_cacheService.Object, _request);

        internal WhenGettingFileUploadCacheDeleteFixture WithRedirectTo(FileUploadCacheDeleteRedirect? redirectTo)
        {
            _request.RedirectTo = redirectTo;
            return this;
        }

        internal void VerifyCacheIsCleared()
        {
            _cacheService.Verify(x => x.ClearCache(_request.CacheRequestId.ToString()), Times.Once);
        }
    }
}