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
    public class WhenGettingToReviewApprentices
    {
        [Test]
        public async Task ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingToReviewApprenticesFixture();

            //Act
            var result = await fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_ReviewApprenticeViewModel()
        {
            //Arrange
            var fixture = new WhenGettingToReviewApprenticesFixture();

            //Act
            var viewResult = await fixture.Act();

            //Assert
            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadReviewApprenticeViewModel>();
            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingToReviewApprenticesFixture
    {
        private CohortController _sut { get; set; }        

        private readonly FileUploadReviewApprenticeRequest _request;
        private readonly long _providerId = 123;
        private readonly string _cohortRef = "VLB8N4";
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadReviewApprenticeViewModel _viewModel;

        public WhenGettingToReviewApprenticesFixture()
        {
            var fixture = new AutoFixture.Fixture();

            _viewModel = fixture.Create<FileUploadReviewApprenticeViewModel>();
            _request = new FileUploadReviewApprenticeRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId, CohortRef = _cohortRef };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadReviewApprenticeViewModel>(_request)).ReturnsAsync(_viewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadReviewApprentices(_request);
    }
}
