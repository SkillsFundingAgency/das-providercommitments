using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadStart
    {
        [Test]
        public async Task Then_Returns_View()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task Then_ProviderId_Is_Mapped()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadValidateViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }

        [Test]
        public async Task The_Errors_Are_Mapped()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

            await fixture.Act();

            fixture.VerifyErrorsAreMapped();
        }

        [Test]
        public async Task When_NorErrors_RedirectTo_FileUploadStart()
        {
            var fixture = new WhenGettingFileUploadStartFixture();

           var result = await fixture.SetUpNoErrors().Act();

            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadStart");
        }
    }

    public class WhenGettingFileUploadStartFixture
    {
        public CohortController Sut { get; set; }

        private readonly FileUploadValidateErrorRequest _request;
        public readonly BulkUploadValidateApiResponse _errors;
        private readonly FileUploadValidateViewModel _viewModel;
        public readonly long ProviderId = 123;
        private readonly Mock<IModelMapper> _mapper;
        private readonly TempDataDictionary _tempData;

        public WhenGettingFileUploadStartFixture()
        {
            var fixture = new Fixture();
            _errors = fixture.Create<BulkUploadValidateApiResponse>();
            _viewModel = fixture.Create<FileUploadValidateViewModel>();
            
            _mapper = new Mock<IModelMapper>();
            _mapper.Setup(x => x.Map<FileUploadValidateViewModel>(It.IsAny<BulkUploadValidateApiResponse>())).ReturnsAsync(() => _viewModel);
            
            _request = new FileUploadValidateErrorRequest { ProviderId = ProviderId };
            
            Sut = new CohortController(Mock.Of<IMediator>(), _mapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>());

            _tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _tempData.Put(Constants.BulkUpload.BulkUploadErrors, _errors);
            Sut.TempData = _tempData;
        }

        public async Task<IActionResult> Act() => await Sut.FileUploadValidationErrors(_request);

        internal void VerifyErrorsAreMapped()
        {
            _mapper.Verify(x => x.Map<FileUploadValidateViewModel>(It.IsAny<BulkUploadValidateApiResponse>()), Times.Once);
        }

        internal WhenGettingFileUploadStartFixture SetUpNoErrors()
        {
            _tempData.Remove(Constants.BulkUpload.BulkUploadErrors);
            return this;
        }
    }
}