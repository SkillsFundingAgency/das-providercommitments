using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadValidationError
    {
        [Test]
        public async Task Then_Returns_View()
        {
            var fixture = new WhenGettingFileUploadValidationErrorFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task Then_ProviderId_Is_Mapped()
        {
            var fixture = new WhenGettingFileUploadValidationErrorFixture();

            var viewResult = await fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadValidateViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }
    }

    public class WhenGettingFileUploadValidationErrorFixture
    {
        public CohortController Sut { get; set; }

        private readonly FileUploadValidateErrorRequest _request;
        public readonly List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError> _errors;
        private readonly FileUploadValidateViewModel _viewModel;
        public readonly long ProviderId = 123;
        private readonly Mock<IModelMapper> _mapper;
        private readonly TempDataDictionary _tempData;

        public WhenGettingFileUploadValidationErrorFixture()
        {
            var fixture = new Fixture();
            _errors = fixture.Create<List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError>>();
            _viewModel = fixture.Build<FileUploadValidateViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>())
                .With(x => x.ProviderId, ProviderId)
                .Create();

            _request = new FileUploadValidateErrorRequest { ProviderId = ProviderId };

            _mapper = new Mock<IModelMapper>();
            _mapper.Setup(x => x.Map<FileUploadValidateViewModel>(_request)).ReturnsAsync(() => _viewModel);
            
            Sut = new CohortController(Mock.Of<IMediator>(), _mapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), 
                        Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>());

            _tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _tempData.Put(Constants.BulkUpload.BulkUploadErrors, _errors);
            Sut.TempData = _tempData;
        }

        public async Task<IActionResult> Act() => await Sut.FileUploadValidationErrors(_request);

        internal void VerifyErrorsAreMapped()
        {
            _mapper.Verify(x => x.Map<FileUploadValidateViewModel>(It.IsAny<BulkUploadValidateApiResponse>()), Times.Once);
        }

        internal WhenGettingFileUploadValidationErrorFixture SetUpNoErrors()
        {
            _tempData.Remove(Constants.BulkUpload.BulkUploadErrors);
            return this;
        }
    }
}