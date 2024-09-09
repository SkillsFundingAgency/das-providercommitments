using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;

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
            Assert.That(model, Is.Not.Null);
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
        private readonly CohortController _sut;
        private const long ProviderId = 123;
        private readonly Mock<IModelMapper> _modelMapper;

        public WhenGettingBulkUploadAddAndApproveDraftApprenticeshipsFixture()
        {
            var fixture = new Fixture();
            var draftApprenticeshipViewModel = fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>();
            draftApprenticeshipViewModel.ProviderId = 123;

            _modelMapper = new Mock<IModelMapper>();
            BulkUploadAddAndApproveDraftApprenticeshipsResponse response = DraftApprenticeshipsResponse();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, response);

            _modelMapper.Setup(x => x.Map<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>(It.IsAny<BulkUploadAddAndApproveDraftApprenticeshipsResponse>()))
                .ReturnsAsync(draftApprenticeshipViewModel);

            _sut = new CohortController(Mock.Of<IMediator>(), _modelMapper.Object, Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(),
                Mock.Of<IEncodingService>(), Mock.Of<IOuterApiService>(),Mock.Of<IAuthorizationService>());

            _sut.TempData = tempData;
        }

        public Task<IActionResult> Act() => _sut.FileUploadSuccess(ProviderId);

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
                       new()
                       {
                           CohortReference = "MKRK7V",
                           EmployerName = "Tesco",
                           NumberOfApprenticeships = 1
                       },
                      new()
                      {
                           CohortReference = "MKRK7V",
                           EmployerName = "Tesco",
                           NumberOfApprenticeships = 1
                      },
                      new()
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
