using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadReviewDelete
    {
        [TestCase(null)]
        [TestCase(FileUploadReviewDeleteRedirect.UploadAgain)]
        public async Task Then_Redirects_To_FileUploadStart(FileUploadReviewDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            var result =  await fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadStart");
        }


        [TestCase(FileUploadReviewDeleteRedirect.Home)]
        public async Task Then_Redirects_To_Home(FileUploadReviewDeleteRedirect? redirectTo)
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            var result = await fixture.WithRedirectTo(redirectTo).Act();

            result.VerifyReturnsRedirect().WithUrl("pasurl/account");
        }

        [Test]
        public async Task Then_Cache_Is_Cleared()
        {
            var fixture = new WhenGettingFileUploadReviewDeleteFixture();

            await fixture.Act();

            fixture.VerifyCacheIsCleared();
        }
    }

    public class WhenGettingFileUploadReviewDeleteFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileUploadReviewDeleteRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadReviewViewModel _viewModel;
        private readonly Mock<ILinkGenerator> _linkGenerator;
        private readonly Mock<IMediator> _mediator;

        public WhenGettingFileUploadReviewDeleteFixture()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Create<FileUploadReviewViewModel>();
            _request = new FileUploadReviewDeleteRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadReviewViewModel>(_request)).ReturnsAsync(_viewModel);
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<DeleteCachedFileCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => Unit.Value);


           _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(x => x.ProviderApprenticeshipServiceLink("/account")).Returns("pasurl/account");
        
            _sut = new CohortController(_mediator.Object, _modelMapper.Object, _linkGenerator.Object, Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(), Mock.Of<RecognitionOfPriorLearningConfiguration>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReviewDelete(_request);

        internal WhenGettingFileUploadReviewDeleteFixture WithRedirectTo(FileUploadReviewDeleteRedirect? redirectTo)
        {
            _request.RedirectTo = redirectTo;
            return this;
        }

        internal void VerifyCacheIsCleared()
        {
            _mediator.Verify(x => x.Send(It.IsAny<DeleteCachedFileCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}