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
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadReview
    {
        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_RedirectToReview()
        {
            var fixture = new WhenIPostFileUploadReviewFixture();

            var result = await fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("SuccessSaveDraft"); ;
        }

        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_CohortsAreCreated()
        {
            var fixture = new WhenIPostFileUploadReviewFixture();

            await fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            fixture.VerifyCohortsAreCreated();
        }


        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_MapperIsCalled()
        {
            var fixture = new WhenIPostFileUploadReviewFixture();

            await fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            fixture.VerifyMapperIsCalled();
        }

        [Test]
        public async Task When_SelectedOption_Is_UploadAnAmendedFile_RedirectTo_FileUploadStart()
        {
            var fixture = new WhenIPostFileUploadReviewFixture();

            var result = await fixture.WithSelectedOption(FileUploadReviewOption.UploadAmendedFile).Act();
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadAmendedFile");
        }
    }

    public class WhenIPostFileUploadReviewFixture
    {
        public CohortController Sut { get; set; }

        public string RedirectUrl;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<ICommitmentsApiClient> _commitmentApiClient;
        private readonly FileUploadReviewViewModel _viewModel;
        private readonly BulkUploadAddDraftApprenticeshipsRequest _apiRequest;

        public WhenIPostFileUploadReviewFixture()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Create<FileUploadReviewViewModel>();
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(_viewModel)).ReturnsAsync(() => _apiRequest);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            Sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, Mock.Of<ILinkGenerator>(), _commitmentApiClient.Object, Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
            Sut.TempData = tempData;
        }

        public WhenIPostFileUploadReviewFixture WithSelectedOption(FileUploadReviewOption selectedOption)
        {
            _viewModel.SelectedOption = selectedOption;
            return this;
        }

        public void VerifyCohortsAreCreated()
        {
            _commitmentApiClient.Verify(x => x.BulkUploadDraftApprenticeships(_viewModel.ProviderId, _apiRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void VerifyMapperIsCalled()
        {
            _mockModelMapper.Verify(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(_viewModel), Times.Once);
        }


        public async Task<IActionResult> Act() => await Sut.FileUploadReview(_viewModel);
    }
}
