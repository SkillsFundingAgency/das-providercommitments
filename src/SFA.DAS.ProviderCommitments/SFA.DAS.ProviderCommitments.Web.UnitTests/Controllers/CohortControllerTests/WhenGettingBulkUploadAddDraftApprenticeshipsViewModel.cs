using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task Then_BulkUploadDraftApprenticeships_IsCalled()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddDraftApprenticeshipsFixture();

            //Act
            await fixture.Act();

            //Assert
            fixture.VerifyCohortsAreCreated();
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

        private readonly FileUploadReviewViewModel fileUploadViewModel;
        private readonly BulkUploadAddDraftApprenticeshipsRequest draftApprenticeshipsRequest;
        private readonly BulkUploadAddDraftApprenticeshipsViewModel draftApprenticeshipViewModel;
        private readonly long providerId = 123;
        private Mock<IModelMapper> _modelMapper;
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;

        public WhenGettingBulkUploadAddDraftApprenticeshipsFixture()
        {
            var fixture = new Fixture();
            fileUploadViewModel = fixture.Create<FileUploadReviewViewModel>();
            fileUploadViewModel.ProviderId = providerId;
            draftApprenticeshipsRequest = fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            draftApprenticeshipViewModel = fixture.Create<BulkUploadAddDraftApprenticeshipsViewModel>();

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(fileUploadViewModel)).ReturnsAsync(draftApprenticeshipsRequest);
            GetBulkUploadAddDraftApprenticeshipsResponse response = DraftApprenticeshipsResponse();

            _modelMapper.Setup(x => x.Map<BulkUploadAddDraftApprenticeshipsViewModel>(response)).ReturnsAsync(draftApprenticeshipViewModel);

            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.BulkUploadDraftApprenticeships(providerId, draftApprenticeshipsRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), _commitmentsApiClient.Object,
                Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }
        
        public Task<IActionResult> Act() => _sut.SuccessSaveDraft(fileUploadViewModel);

        public void VerifyCohortsAreCreated()
        {
            _commitmentsApiClient.Verify(m => m.BulkUploadDraftApprenticeships(It.IsAny<long>(), It.IsAny<BulkUploadAddDraftApprenticeshipsRequest>(),  
                    It.IsAny<CancellationToken>()), Times.Once());
        }

        public void VerifyMapperIsCalled()
        {
            _modelMapper.Verify(x => x.Map<BulkUploadAddDraftApprenticeshipsRequest>(fileUploadViewModel), Times.Once);
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
