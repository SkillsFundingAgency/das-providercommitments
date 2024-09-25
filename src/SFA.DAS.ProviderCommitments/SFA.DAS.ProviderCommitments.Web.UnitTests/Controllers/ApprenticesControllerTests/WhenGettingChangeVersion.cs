using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenGettingChangeVersion
{
    private GetChangeVersionFixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new GetChangeVersionFixture();
    }

    [Test]
    public async Task ThenVerifyMapperWasCalled()
    {
        await _fixture.ChangeVersion();

        _fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsViewModel()
    {
        var result = await _fixture.ChangeVersion();

        _fixture.VerifyViewModel(result as ViewResult);
    }
}

public class GetChangeVersionFixture
{
    private readonly ApprenticeController Controller;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly ChangeVersionRequest _request;
    private readonly ChangeVersionViewModel _viewModel;

    public GetChangeVersionFixture()
    {
        var fixture = new Fixture();

        _request = fixture.Create<ChangeVersionRequest>();
        _viewModel = fixture.Create<ChangeVersionViewModel>();

        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(m => m.Map<ChangeVersionViewModel>(_request)).ReturnsAsync(_viewModel);

        Controller = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>());
        Controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
    }

    public async Task<IActionResult> ChangeVersion()
    {
        var result = await Controller.ChangeVersion(_request);

        return result as ViewResult;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(m => m.Map<ChangeVersionViewModel>(_request));
    }

    public void VerifyViewModel(ViewResult viewResult)
    {
        var viewModel = viewResult.Model as ChangeVersionViewModel;

        viewModel.Should().Be(_viewModel);
    }
}