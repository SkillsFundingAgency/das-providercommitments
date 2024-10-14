using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenGettingChangeOption
{
    private GetChangeOptionFixture _fixture;

    [SetUp]
    public void Arrange() => _fixture = new GetChangeOptionFixture();

    [Test]
    public async Task ThenVerifyMapperWasCalled()
    {
        await _fixture.ChangeOption();

        _fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsViewModel()
    {
        var result = await _fixture.ChangeOption();

        _fixture.VerifyViewModel(result as ViewResult);
    }
}

public class GetChangeOptionFixture
{
    private readonly ApprenticeController _controller;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly ChangeOptionRequest _request;
    private readonly ChangeOptionViewModel _viewModel;

    public GetChangeOptionFixture()
    {
        var fixture = new Fixture();

        _request = fixture.Create<ChangeOptionRequest>();
        _viewModel = fixture.Create<ChangeOptionViewModel>();

        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(m => m.Map<ChangeOptionViewModel>(_request)).ReturnsAsync(_viewModel);

        _controller = new ApprenticeController(_modelMapperMock.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            Mock.Of<ICommitmentsApiClient>(), 
            Mock.Of<IOuterApiService>(), 
            Mock.Of<ICacheStorageService>());
    }

    public async Task<IActionResult> ChangeOption()
    {
        var result = await _controller.ChangeOption(_request);

        return result as ViewResult;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(m => m.Map<ChangeOptionViewModel>(_request));
    }

    public void VerifyViewModel(ViewResult viewResult)
    {
        var viewModel = viewResult.Model as ChangeOptionViewModel;

        viewModel.Should().Be(_viewModel);
    }
}