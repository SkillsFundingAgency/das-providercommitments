using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenIPostFileUploadReview
    {
        private  WhenIPostFileUploadReviewFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenIPostFileUploadReviewFixture();
        }

        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_RedirectToReview()
        {
            //Act
            var result = await _fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            
            //Assert
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadSuccessSaveDraft"); ;
        }

        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_CohortsAreCreated()
        {
            //Act
            await _fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            
            //Assert
            _fixture.VerifyCohortsAreCreated();
        }


        [Test]
        public async Task When_SelectedOption_Is_SaveButDontSendToEmployer_MapperIsCalled()
        {
            //Act
            await _fixture.WithSelectedOption(FileUploadReviewOption.SaveButDontSend).Act();
            
            //Assert
            _fixture.VerifyMapperIsCalled();
        }

        // TODO re-add these tests when the Add/Approve feature is turned back on
        //[Test]
        //public async Task When_SelectedOption_Is_ApproveAndSendToEmployer_RedirectToReview()
        //{
        //    //Act
        //    var result = await fixture.WithSelectedOption(FileUploadReviewOption.ApproveAndSend).Act();
            
        //    //Aseert
        //    result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadSuccess"); ;
        //}

        //[Test]
        //public async Task When_SelectedOption_Is_ApproveAndSendToEmployer_CohortsAreCreated()
        //{
        //    //Act
        //    await fixture.WithSelectedOption(FileUploadReviewOption.ApproveAndSend).Act();
            
        //    //Assert
        //    fixture.VerifyCohortsAreCreatedAndSendToEmployer();
        //}

        //[Test]
        //public async Task When_SelectedOption_Is_ApproveAndSendToEmployer_MapperIsCalled()
        //{
        //    //Act
        //    await fixture.WithSelectedOption(FileUploadReviewOption.ApproveAndSend).Act();

        //    //Assert
        //    fixture.VerifyApproveAndSendToEmployerMapperIsCalled();
        //}

        [Test]
        public async Task When_SelectedOption_Is_UploadAnAmendedFile_RedirectTo_FileUploadStart()
        {
            //Act
            var result = await _fixture.WithSelectedOption(FileUploadReviewOption.UploadAmendedFile).Act();
            
            //Assert
            result.VerifyReturnsRedirectToActionResult().WithActionName("FileUploadAmendedFile");
        }   
    }

    public class WhenIPostFileUploadReviewFixture
    {
        private readonly CohortController _sut;
        private readonly Mock<IModelMapper> _mockModelMapper;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly FileUploadReviewViewModel _viewModel;        
        private readonly BulkUploadAddDraftApprenticeshipsRequest _apiRequest;

        public WhenIPostFileUploadReviewFixture()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Create<FileUploadReviewViewModel>();
            _outerApiService = new Mock<IOuterApiService>();
            

            _apiRequest = fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            var addAndApproveApiRequest = fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsRequest>();
            var bulkUploadAddAndApproveDraftApprenticeshipsResult = fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsResult>();

            _mockModelMapper = new Mock<IModelMapper>();
            _mockModelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(_viewModel)).ReturnsAsync(() => _apiRequest);
            _mockModelMapper.Setup(x => x.Map<BulkUploadAddAndApproveDraftApprenticeshipsRequest>(_viewModel)).ReturnsAsync(() => addAndApproveApiRequest);

            _outerApiService.Setup(x => x.BulkUploadAddAndApproveDraftApprenticeships(addAndApproveApiRequest)).ReturnsAsync(bulkUploadAddAndApproveDraftApprenticeshipsResult);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _sut = new CohortController(Mock.Of<IMediator>(), _mockModelMapper.Object, Mock.Of<ILinkGenerator>(),Mock.Of<ICommitmentsApiClient>(), 
                         Mock.Of<IEncodingService>(), _outerApiService.Object,Mock.Of<IAuthorizationService>());
            _sut.TempData = tempData;
        }

        public WhenIPostFileUploadReviewFixture WithSelectedOption(FileUploadReviewOption selectedOption)
        {
            _viewModel.SelectedOption = selectedOption;
            return this;
        }

        public void VerifyCohortsAreCreated()
        {
            _outerApiService.Verify(x => x.BulkUploadDraftApprenticeships(_apiRequest), Times.Once);
        }

        public void VerifyMapperIsCalled()
        {
            _mockModelMapper.Verify(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(_viewModel), Times.Once);
        }
      
        public async Task<IActionResult> Act() => await _sut.FileUploadReview(_viewModel);
    }
}
