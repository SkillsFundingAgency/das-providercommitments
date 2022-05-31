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
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingBulkUploadAddDraftApprenticeshipsViewModel
    {
        [Test]
        public async Task ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddDraftApprenticeshipsFixture();

            //Act
            var result = await fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_BulkUploadAddDraftApprenticeshipsViewModel()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddDraftApprenticeshipsFixture();

            //Act
            var viewResult = await fixture.Act();
            var model = viewResult.VerifyReturnsViewModel().WithModel<BulkUploadAddDraftApprenticeshipsViewModel>();

            //Assert
            Assert.IsNotNull(model);
        }

        [Test]
        public async Task Then_SelectedOption_Is_SaveButDontSendToEmployer_MapperIsCalled()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddDraftApprenticeshipsFixture();

            //Act
            await fixture.Act();

            //Assert            
            fixture.VerifyMapperIsCalled();
        }
    }    

    public class WhenGettingBulkUploadAddDraftApprenticeshipsFixture
    {
        private CohortController _sut { get; set; }

        private readonly BulkUploadAddDraftApprenticeshipsViewModel draftApprenticeshipViewModel;
        private readonly long providerId = 123;
        private Mock<IModelMapper> _modelMapper;
        private readonly TempDataDictionary _tempData;

        public WhenGettingBulkUploadAddDraftApprenticeshipsFixture()
        {
            var fixture = new Fixture();
            draftApprenticeshipViewModel = fixture.Create<BulkUploadAddDraftApprenticeshipsViewModel>();            
            draftApprenticeshipViewModel.ProviderId = 123;

            _modelMapper = new Mock<IModelMapper>();
            GetBulkUploadAddDraftApprenticeshipsResponse response = DraftApprenticeshipsResponse();

            _tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _tempData.Put(Constants.BulkUpload.DraftApprenticeshipResponse, response);
            
            _modelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsViewModel>(It.IsAny<GetBulkUploadAddDraftApprenticeshipsResponse>()))
                .ReturnsAsync(draftApprenticeshipViewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(),Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IAuthorizationService>(), Mock.Of<IEncodingService>(),  Mock.Of<IOuterApiService>(), Mock.Of<RecognitionOfPriorLearningConfiguration>());

            _sut.TempData = _tempData;
        }
        
        public Task<IActionResult> Act() => _sut.FileUploadSuccessSaveDraft(providerId);

        public void VerifyMapperIsCalled()
        {
            _modelMapper.Verify(x => x.Map<BulkUploadAddDraftApprenticeshipsViewModel>(It.IsAny<GetBulkUploadAddDraftApprenticeshipsResponse>()), Times.Once);
        }

        private static GetBulkUploadAddDraftApprenticeshipsResponse DraftApprenticeshipsResponse()
        {
            return new GetBulkUploadAddDraftApprenticeshipsResponse()
            {
                BulkUploadAddDraftApprenticeshipsResponse = new List<BulkUploadAddDraftApprenticeshipsResponse>()
                   {
                       new BulkUploadAddDraftApprenticeshipsResponse()
                       {
                           CohortReference = "MKRK7V",
                           EmployerName = "Tesco",
                           NumberOfApprenticeships = 1
                       },
                      new BulkUploadAddDraftApprenticeshipsResponse()
                      {
                           CohortReference = "MKRK7V",
                           EmployerName = "Tesco",
                           NumberOfApprenticeships = 1
                      },
                      new BulkUploadAddDraftApprenticeshipsResponse()
                      {
                           CohortReference = "MKRK7N",
                           EmployerName = "Nasdaq",
                           NumberOfApprenticeships = 1
                      },
                   }
            };
        }

    }
}
