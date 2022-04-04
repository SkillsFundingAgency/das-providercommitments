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
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsViewModel
    {
        [Test]
        public async Task ThenReturnsView()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture();

            //Act
            var result = await fixture.Act();

            //Assert
            result.VerifyReturnsViewModel();
        }

        [Test]
        public async Task ThenReturnsView_With_BulkUploadAddAndApproveDraftApprenticeshipsViewModel()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture();

            //Act
            var viewResult = await fixture.Act();
            var model = viewResult.VerifyReturnsViewModel().WithModel<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>();

            //Assert
            Assert.IsNotNull(model);
        }

        [Test]
        public async Task Then_SelectedOption_Is_SaveAndSendToEmployer_MapperIsCalled()
        {
            //Arrange
            var fixture = new WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture();

            //Act
            await fixture.Act();

            //Assert            
            fixture.VerifyMapperIsCalled();
        }
    }

    public class WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture
    {
        private CohortController _sut { get; set; }

        private readonly BulkUploadAddAndApproveDraftApprenticeshipsViewModel draftApprenticeshipViewModel;
        private readonly long providerId = 123;
        private Mock<IModelMapper> _modelMapper;
        private readonly TempDataDictionary _tempData;

        public WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture()
        {
            var fixture = new Fixture();
            draftApprenticeshipViewModel = fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>();
            draftApprenticeshipViewModel.ProviderId = 123;

            _modelMapper = new Mock<IModelMapper>();
            BulkUploadAddAndApproveDraftApprenticeshipsResponse response = DraftApprenticeshipsResponse();

            _tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _tempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, response);

            _modelMapper.Setup(x => x.Map<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>(It.IsAny<BulkUploadAddAndApproveDraftApprenticeshipsResponse>()))
                .ReturnsAsync(draftApprenticeshipViewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>());
            _sut.TempData = _tempData;
        }

        public Task<IActionResult> Act() => _sut.FileUploadSuccess(providerId);

        public void VerifyMapperIsCalled()
        {
            _modelMapper.Verify(x => x.Map<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>(It.IsAny<BulkUploadAddAndApproveDraftApprenticeshipsResponse>()), Times.Once);
        }

        private static BulkUploadAddAndApproveDraftApprenticeshipsResponse DraftApprenticeshipsResponse()
        {
            return new BulkUploadAddAndApproveDraftApprenticeshipsResponse()
            {
                BulkUploadAddAndApproveDraftApprenticeshipResponse = new List<BulkUploadAddDraftApprenticeshipsResponse>()
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
