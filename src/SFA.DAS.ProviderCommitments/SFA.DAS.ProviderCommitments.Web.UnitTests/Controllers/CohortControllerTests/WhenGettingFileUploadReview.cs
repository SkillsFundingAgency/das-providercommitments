using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadReview
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadReviewFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_FileUploadReviewViewModel()
        {
            var fixture = new WhenGettingFileUploadReviewFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadReviewViewModel>();

            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingFileUploadReviewFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileUploadReviewRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadReviewViewModel _viewModel;

        public WhenGettingFileUploadReviewFixture()
        {
            var fixture = new AutoFixture.Fixture();
            

            _viewModel = fixture.Create<FileUploadReviewViewModel>();
            _request = new FileUploadReviewRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadReviewViewModel>(_request)).ReturnsAsync(_viewModel);
        
            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReview(_request);
    }
}