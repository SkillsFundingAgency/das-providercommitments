using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;
using static SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class ApprenticeControllerTestFixtureBase
    {
        protected Fixture _autoFixture;

        protected Mock<IModelMapper> _mockMapper;
        protected Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        protected Mock<IOuterApiService> _mockOuterApiService;
        protected Mock<ILinkGenerator> _mockLinkGenerator;
        protected Mock<IUrlHelper> _mockUrlHelper;
        protected Mock<ITempDataDictionary> _mockTempData;
        private IActionResult _actionResult;

        protected readonly ApprenticeController _controller;
        private readonly DetailsViewModel _detailsViewModel;
        private readonly DetailsRequest _detailsRequest;

        public ApprenticeControllerTestFixtureBase()
        {
            _autoFixture = new Fixture();
            _autoFixture.Customize(new DateCustomisation());
            _mockMapper = new Mock<IModelMapper>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockLinkGenerator = new Mock<ILinkGenerator>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockTempData = new Mock<ITempDataDictionary>();
            _mockOuterApiService = new Mock<IOuterApiService>();

            _mockOuterApiService.Setup(x =>
                    x.ValidateChangeOfEmployerOverlap(It.IsAny<ValidateChangeOfEmployerOverlapApimRequest>()))
                .Returns(Task.CompletedTask);

            _detailsRequest = new DetailsRequest { ProviderId = 123, ApprenticeshipId = 456, ApprenticeshipHashedId = "XYZ" };
            _detailsViewModel = _autoFixture.Create<DetailsViewModel>();

            _mockMapper
               .Setup(x => x.Map<DetailsViewModel>(It.IsAny<DetailsRequest>()))
               .ReturnsAsync(_detailsViewModel);

            _controller = new ApprenticeController(_mockMapper.Object,
                Mock.Of<ICookieStorageService<IndexRequest>>(),
                _mockCommitmentsApiClient.Object,
                _mockOuterApiService.Object);

            _controller.Url = _mockUrlHelper.Object;
            _controller.TempData = _mockTempData.Object;
        }

        public async Task<ApprenticeControllerTestFixtureBase> GetDetails()
        {
            _actionResult = await _controller.Details(_detailsRequest);
            return this;
        }

        public ApprenticeControllerTestFixtureBase VerifyModelMapperDetailsRequest_ToViewModelIsCalled()
        {
            _mockMapper.Verify(x => x.Map<DetailsViewModel>(It.IsAny<DetailsRequest>()), Times.Once);
            return this;
        }

        public ApprenticeControllerTestFixtureBase VerifyDetailViewReturned()
        {
            var viewResult = _actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as DetailsViewModel;
            Assert.IsNotNull(model);
            return this;
        }
    }
}