﻿using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadAmendedFile
    {
        [Test]
        public async Task When_User_Selects_To_Upload_AmendedFile()
        {
            var fixture = new WhenIPostFileUploadAmendedFileFixture();

            var result = await fixture.WithSelectedOption(true).Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadStart"); 
        }

        [Test]
        public async Task When_User_Selects_Not_To_Upload_AmendedFile()
        {
            var fixture = new WhenIPostFileUploadAmendedFileFixture();

            var result = await fixture.WithSelectedOption(false).Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadReview");
        }


        [Test]
        public async Task When_User_Selects_To_Upload_AmendedFile_Previous_Cached_File_Is_Deleted()
        {
            var fixture = new WhenIPostFileUploadAmendedFileFixture();

            await fixture.WithSelectedOption(true).Act();
            fixture.VerifyCachedFileIsDeleted();
        }
    }

    public class WhenIPostFileUploadAmendedFileFixture
    {
        private readonly CohortController _sut;
        private readonly FileUploadAmendedFileViewModel _viewModel;
        private readonly Mock<IMediator> _mockMediator;

        public WhenIPostFileUploadAmendedFileFixture()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Create<FileUploadAmendedFileViewModel>();
            var commitmentApiClient = new Mock<ICommitmentsApiClient>();

            var mockModelMapper = new Mock<IModelMapper>();
            _mockMediator = new Mock<IMediator>();

            _sut = new CohortController(_mockMediator.Object, mockModelMapper.Object, Mock.Of<ILinkGenerator>(), commitmentApiClient.Object, 
                        Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>(), Mock.Of<ILogger<CohortController>>());
        }

        public WhenIPostFileUploadAmendedFileFixture WithSelectedOption(bool selectedOption)
        {
            _viewModel.Confirm = selectedOption;
            return this;
        }

        public void VerifyCachedFileIsDeleted()
        {
            _mockMediator.Verify(x => x.Send(It.IsAny<DeleteCachedFileCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        public async Task<IActionResult> Act() => await _sut.FileUploadAmendedFile(_viewModel);
    }
}
