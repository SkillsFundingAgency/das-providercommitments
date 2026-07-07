using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenGettingAllChangeHistory
{
    private WhenGettingAllChangeHistoryFixture _fixture;

    [SetUp]
    public void Arrange() => _fixture = new WhenGettingAllChangeHistoryFixture();

    [Test]
    public async Task ThenVerifyMapperWasCalled()
    {
        await _fixture.GetAllChangeHistory();

        _fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsViewModel()
    {
        var result = await _fixture.GetAllChangeHistory();

        _fixture.VerifyViewModel(result as ViewResult);
    }
}

public class WhenGettingAllChangeHistoryFixture
{
    private readonly ApprenticeController _controller;
    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly GetAllChangeHistoryRequest _request;
    private readonly GetAllChangeHistoryListViewModel _viewModel;

    public WhenGettingAllChangeHistoryFixture()
    {
        var fixture = new Fixture();

        _request = fixture.Create<GetAllChangeHistoryRequest>();
        _viewModel = fixture.Create<GetAllChangeHistoryListViewModel>();

        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(m => m.Map<GetAllChangeHistoryListViewModel>(_request)).ReturnsAsync(_viewModel);

        _controller = new ApprenticeController(_modelMapperMock.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IOuterApiService>(),
            Mock.Of<ICacheStorageService>());
    }

    public async Task<IActionResult> GetAllChangeHistory()
    {
        var result = await _controller.GetAllChangeHistory(_request);

        return result as ViewResult;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapperMock.Verify(m => m.Map<GetAllChangeHistoryListViewModel>(_request));
    }

    public void VerifyViewModel(ViewResult viewResult)
    {
        var viewModel = viewResult.Model as GetAllChangeHistoryListViewModel;

        viewModel.Should().Be(_viewModel);
    }
}