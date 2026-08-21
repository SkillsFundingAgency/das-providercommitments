using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenGettingAndPostingInvalidIlrChanges
{
    [Test]
    public async Task Get_ThenReturnsTheMappedView()
    {
        var fixture = new WhenGettingAndPostingInvalidIlrChangesFixture();

        var result = await fixture.Get();

        fixture.VerifyGetMappedView(result);
    }

    [Test]
    public async Task Post_ThenAcknowledgesAndRedirectsToDetails()
    {
        var fixture = new WhenGettingAndPostingInvalidIlrChangesFixture();

        var result = await fixture.PostValid();

        fixture.VerifyAcknowledgedAndRedirectedToDetails(result);
    }

    [Test]
    public async Task Post_ThenRedisplaysWhenValidationFails()
    {
        var fixture = new WhenGettingAndPostingInvalidIlrChangesFixture();

        var result = await fixture.PostInvalid();

        fixture.VerifyRedisplayedOnValidationFailure(result);
    }
}

public class WhenGettingAndPostingInvalidIlrChangesFixture
{
    private readonly ApprenticeController _controller;
    private readonly Mock<IModelMapper> _modelMapper;
    private readonly InvalidIlrChangesRequest _request;
    private readonly InvalidIlrChangesViewModel _viewModel;
    private readonly InvalidIlrChangesViewModel _refreshedViewModel;

    public WhenGettingAndPostingInvalidIlrChangesFixture()
    {
        var autoFixture = new Fixture();
        _request = autoFixture.Create<InvalidIlrChangesRequest>();
        _viewModel = autoFixture.Create<InvalidIlrChangesViewModel>();
        _refreshedViewModel = autoFixture.Create<InvalidIlrChangesViewModel>();

        _modelMapper = new Mock<IModelMapper>();
        _modelMapper.Setup(m => m.Map<InvalidIlrChangesViewModel>(_request)).ReturnsAsync(_viewModel);
        _modelMapper.Setup(m => m.Map<InvalidIlrChangesViewModel>(It.IsAny<InvalidIlrChangesRequest>()))
            .ReturnsAsync(_refreshedViewModel);
        _modelMapper.Setup(m => m.Map<InvalidIlrChangesAcknowledgementResult>(It.IsAny<InvalidIlrChangesViewModel>()))
            .ReturnsAsync(new InvalidIlrChangesAcknowledgementResult());

        _controller = new ApprenticeController(
            _modelMapper.Object,
            Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IOuterApiService>(),
            Mock.Of<ICacheStorageService>());
    }

    public Task<IActionResult> Get()
    {
        _modelMapper.Setup(m => m.Map<InvalidIlrChangesViewModel>(_request)).ReturnsAsync(_viewModel);
        return _controller.InvalidIlrChanges(_request);
    }

    public Task<IActionResult> PostValid()
    {
        return _controller.InvalidIlrChanges(_viewModel);
    }

    public Task<IActionResult> PostInvalid()
    {
        _controller.ModelState.AddModelError("RequestSets[0].DeleteAlert", "Select if you would like to delete this notification and alert");
        return _controller.InvalidIlrChanges(_viewModel);
    }

    public void VerifyGetMappedView(IActionResult result)
    {
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(_viewModel);
    }

    public void VerifyAcknowledgedAndRedirectedToDetails(IActionResult result)
    {
        _modelMapper.Verify(m => m.Map<InvalidIlrChangesAcknowledgementResult>(_viewModel), Times.Once);
        var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
        redirect.RouteName.Should().Be(RouteNames.ApprenticeDetail);
        redirect.RouteValues!["ProviderId"].Should().Be(_viewModel.ProviderId);
        redirect.RouteValues["ApprenticeshipHashedId"].Should().Be(_viewModel.ApprenticeshipHashedId);
    }

    public void VerifyRedisplayedOnValidationFailure(IActionResult result)
    {
        _modelMapper.Verify(m => m.Map<InvalidIlrChangesAcknowledgementResult>(It.IsAny<InvalidIlrChangesViewModel>()), Times.Never);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(_refreshedViewModel);
    }
}
