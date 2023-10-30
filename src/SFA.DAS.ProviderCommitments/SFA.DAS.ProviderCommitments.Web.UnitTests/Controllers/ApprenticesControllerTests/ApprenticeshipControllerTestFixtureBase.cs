using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;
using static SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class ApprenticeControllerTestFixtureBase
    {
        protected Fixture _autoFixture;

        protected Mock<IModelMapper> _mockMapper;
        protected Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        protected Mock<ILinkGenerator> _mockLinkGenerator;
        protected Mock<IUrlHelper> _mockUrlHelper;
        protected Mock<ITempDataDictionary> _mockTempData;

        protected readonly ApprenticeController _controller;

        public ApprenticeControllerTestFixtureBase()
        {
            _autoFixture = new Fixture();
            _autoFixture.Customize(new DateCustomisation());
            _mockMapper = new Mock<IModelMapper>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockLinkGenerator = new Mock<ILinkGenerator>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _controller = new ApprenticeController(_mockMapper.Object,
                Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(),
                _mockCommitmentsApiClient.Object);

            _controller.Url = _mockUrlHelper.Object;
            _controller.TempData = _mockTempData.Object;
        }
    }
}
