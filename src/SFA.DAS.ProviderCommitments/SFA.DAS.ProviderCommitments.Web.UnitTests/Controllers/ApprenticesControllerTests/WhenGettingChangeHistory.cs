using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenGettingChangeHistory
{
    private WhenGettingChangeHistoryFixture _fixture;

    [SetUp]
    public void Arrange() => _fixture = new WhenGettingChangeHistoryFixture();

    [Test]
    public async Task ThenVerifyMapperWasCalled()
    {
        await _fixture.GetChangeHistory();

        _fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsViewModel()
    {
        var result = await _fixture.GetChangeHistory();

        _fixture.VerifyViewModel(result as ViewResult);
    }
}

public class WhenGettingChangeHistoryFixture
{
    private readonly ApprenticeController _controller;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly ChangeHistoryRequest _request;
    private readonly ChangeHistoryListViewModel _viewModel;

    public WhenGettingChangeHistoryFixture()
    {
        var fixture = new Fixture();

        _request = fixture.Create<ChangeHistoryRequest>();
        _viewModel = fixture.Create<ChangeHistoryListViewModel>();

        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(m => m.Map<ChangeHistoryListViewModel>(_request)).ReturnsAsync(_viewModel);

        _controller = new ApprenticeController(_modelMapperMock.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IOuterApiService>(),
            Mock.Of<ICacheStorageService>());
    }

    public async Task<IActionResult> GetChangeHistory()
    {
        var result = await _controller.ChangeHistory(_request);

        return result as ViewResult;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(m => m.Map<ChangeHistoryListViewModel>(_request));
    }

    public void VerifyViewModel(ViewResult viewResult)
    {
        var viewModel = viewResult.Model as ChangeHistoryListViewModel;

        viewModel.Should().Be(_viewModel);
    }
}