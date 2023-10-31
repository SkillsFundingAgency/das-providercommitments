using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using static SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class ApprenticeControllerTestFixtureBase
    {
        protected readonly Fixture AutoFixture;
        protected readonly Mock<IModelMapper> MockMapper;
        protected readonly Mock<ICommitmentsApiClient> MockCommitmentsApiClient;
        protected readonly Mock<ITempDataDictionary> MockTempData;
        protected readonly ApprenticeController Controller;

        private readonly Mock<IUrlHelper> _mockUrlHelper;

        protected ApprenticeControllerTestFixtureBase()
        {
            AutoFixture = new Fixture();
            AutoFixture.Customize(new DateCustomisation());
            MockMapper = new Mock<IModelMapper>();
            MockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            MockTempData = new Mock<ITempDataDictionary>();

            Controller = new ApprenticeController(MockMapper.Object,
                Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
                MockCommitmentsApiClient.Object);

            Controller.Url = _mockUrlHelper.Object;
            Controller.TempData = MockTempData.Object;
        }
    }
}