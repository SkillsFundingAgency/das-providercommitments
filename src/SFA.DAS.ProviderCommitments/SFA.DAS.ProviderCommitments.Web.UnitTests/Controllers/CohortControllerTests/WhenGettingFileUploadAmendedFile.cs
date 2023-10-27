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
    public class WhenGettingFileUploadAmendedFile
    {
        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadAmendedFileFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_FileUploadAmendedFileViewModel()
        {
            var fixture = new WhenGettingFileUploadAmendedFileFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadAmendedFileViewModel>();

            Assert.IsNotNull(model);
        }
    }

    public class WhenGettingFileUploadAmendedFileFixture
    {
        private CohortController _sut { get; set; }

        private readonly FileUploadAmendedFileRequest _request;
        private readonly long _providerId = 123;
        private readonly Guid _cacheRequestId = Guid.NewGuid();
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly FileUploadAmendedFileViewModel _viewModel;

        public WhenGettingFileUploadAmendedFileFixture()
        {
            var fixture = new AutoFixture.Fixture();
            

            _viewModel = fixture.Create<FileUploadAmendedFileViewModel>();
            _request = new FileUploadAmendedFileRequest { ProviderId = _providerId, CacheRequestId = _cacheRequestId };

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<FileUploadAmendedFileViewModel>(_request)).ReturnsAsync(_viewModel);
            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                        Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>());
        }

        public Task<IActionResult> Act() => _sut.FileUploadAmendedFile(_request);
    }
}