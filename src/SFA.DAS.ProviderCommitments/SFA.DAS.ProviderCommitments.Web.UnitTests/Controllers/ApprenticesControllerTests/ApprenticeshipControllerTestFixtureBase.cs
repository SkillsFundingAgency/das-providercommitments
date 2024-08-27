using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;
using static SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice.EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class ApprenticeControllerTestFixtureBase
{
    protected Fixture AutoFixture;

    protected Mock<IModelMapper> MockMapper;
    protected Mock<ICommitmentsApiClient> MockCommitmentsApiClient;
    protected Mock<IOuterApiService> MockOuterApiService;
    protected Mock<ILinkGenerator> MockLinkGenerator;
    protected Mock<IUrlHelper> MockUrlHelper;
    protected Mock<ITempDataDictionary> MockTempData;
    private IActionResult _actionResult;

    protected readonly ApprenticeController Controller;
    private readonly DetailsViewModel _detailsViewModel;
    private readonly DetailsRequest _detailsRequest;

    public ApprenticeControllerTestFixtureBase()
    {
        AutoFixture = new Fixture();
        AutoFixture.Customize(new DateCustomisation());
        MockMapper = new Mock<IModelMapper>();
        MockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
        MockLinkGenerator = new Mock<ILinkGenerator>();
        MockUrlHelper = new Mock<IUrlHelper>();
        MockTempData = new Mock<ITempDataDictionary>();
        MockOuterApiService = new Mock<IOuterApiService>();

        MockOuterApiService.Setup(x =>
                x.ValidateChangeOfEmployerOverlap(It.IsAny<ValidateChangeOfEmployerOverlapApimRequest>()))
            .Returns(Task.CompletedTask);

        _detailsRequest = new DetailsRequest { ProviderId = 123, ApprenticeshipId = 456, ApprenticeshipHashedId = "XYZ" };
        _detailsViewModel = AutoFixture.Create<DetailsViewModel>();

        MockMapper
            .Setup(x => x.Map<DetailsViewModel>(It.IsAny<DetailsRequest>()))
            .ReturnsAsync(_detailsViewModel);

        Controller = new ApprenticeController(MockMapper.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            MockCommitmentsApiClient.Object,
            MockOuterApiService.Object,
            Mock.Of<ICacheStorageService>());

        Controller.Url = MockUrlHelper.Object;
        Controller.TempData = MockTempData.Object;
    }

    public async Task<ApprenticeControllerTestFixtureBase> GetDetails(ApprenticeDetailsBanners banners = 0)
    {
        _actionResult = await Controller.Details(_detailsRequest, banners);
        return this;
    }

    public ApprenticeControllerTestFixtureBase VerifyModelMapperDetailsRequest_ToViewModelIsCalled()
    {
        MockMapper.Verify(x => x.Map<DetailsViewModel>(It.IsAny<DetailsRequest>()), Times.Once);
        return this;
    }

    public ApprenticeControllerTestFixtureBase VerifyDetailViewReturned()
    {
        var viewResult = _actionResult as ViewResult;
        Assert.That(viewResult, Is.Not.Null);
        var model = viewResult.Model as DetailsViewModel;
        Assert.That(model, Is.Not.Null);
        return this;
    }

    public ApprenticeControllerTestFixtureBase VerifyBannerFlagsAreMapped(ApprenticeDetailsBanners expectedBanners)
    {
        var viewResult = _actionResult as ViewResult;
        Assert.That(viewResult, Is.Not.Null);
        viewResult.Should().NotBeNull();
        var model = viewResult.Model as DetailsViewModel;
        model.Should().NotBeNull();
        model.ShowBannersFlags.Should().Be(expectedBanners);
        return this;
    }
}