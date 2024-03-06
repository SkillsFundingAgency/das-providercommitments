using System.Linq;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.Services;

[TestFixture]
[Parallelizable]
public class AuthorizationServiceWithDefaultHandlerTests
{
    private AuthorizationServiceWithDefaultHandlerTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new AuthorizationServiceWithDefaultHandlerTestsFixture();

    [Test]
    public async Task IsAuthorizedAsync_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        _fixture.SetAuthorizedOptions();

        var result = await _fixture.IsAuthorizedAsync();

        result.Should().BeTrue();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationIsAuthorized_ThenShouldReturnValidAuthorizationResult()
    {
        _fixture.SetAuthorizedOptions();

        var result = await _fixture.GetAuthorizationResultAsync();

        result.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public void IsAuthorized_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        _fixture.SetAuthorizedOptions();

        var resut = _fixture.IsAuthorized();

        resut.Should().BeTrue();
    }

    [Test]
    public async Task IsAuthorizedAsync_WhenOperationIsUnauthorized_ThenShouldReturnFalse()
    {
        _fixture.SetUnauthorizedOptions();

        var result = await _fixture.IsAuthorizedAsync();

        result.Should().BeFalse();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationIsUnauthorized_ThenShouldReturnErrorFromAuthorizationAndDefaultAuthorizationService()
    {
        _fixture.SetUnauthorizedOptions();

        var result = await _fixture.GetAuthorizationResultAsync();

        result.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => r2.IsAuthorized == false
                                                  && r2.Errors.Count() == 2
                                                  && r2.HasError<TestTier2UserAccesNotGranted>()
                                                  && r2.HasError<TestEmployerUserRoleNotAuthorized>());
    }

    [Test]
    public async Task IsAuthorizedAsync_WhenDefaultHandlerIsUnauthorized_ThenShouldReturnFalse()
    {
        _fixture.SetUnauthorizedOptionsforDefaultHandler();

        var result = await _fixture.IsAuthorizedAsync();

        result.Should().BeFalse();
    }


    [Test]
    public void GetAuthorizationResult_WhenOperationisAuthorized_ThenVerifyAuthorizationServiceCalledOnce()
    {
        _fixture.SetAuthorizationResult();

        _fixture.GetAuthorizationResult();

        _fixture.VerifyAuthorizationService();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationIsAuthorized_ThenVerifyAuthorizationResultAsyncServiceCalledOnce()
    {
        _fixture.SetAuthorizationResultAsync();

        await _fixture.GetAuthorizationResultAsync();

        _fixture.VerifyAuthorizationResultAsyncService();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationisUnAuthorized_ThenVerifyAuthorizationResultAsyncServiceCalledOnce()
    {
        _fixture.SetAuthorizationResultAsyncWithErrors();

        await _fixture.GetAuthorizationResultAsync();

        _fixture.VerifyAuthorizationResultAsyncService();
    }

    [Test]
    public async Task AuthorizeAsync_WhenOperationisAuthorized_ThenVerifyAuthorizeAsyncServiceCalledOnce()
    {
        _fixture.SetAuthorizeAsync();

        await _fixture.AuthorizeAsync();

        _fixture.VerifyAuthorizeAsyncService();
    }

    [Test]
    public async Task AuthorizeAsync_WhenOperationisUnAuthorized_ThenVerifyAuthorizeAsyncServiceCalledOnce()
    {
        _fixture.SetAuthorizeAsyncReturnsFalse();

        await _fixture.AuthorizeAsync();

        _fixture.VerifyAuthorizeAsyncService();
    }

    [Test]
    public void Authorize_WhenOperationisAuthorized_ThenVerifyAuthorizeCalledOnce()
    {
        _fixture.Authorize();

        _fixture.VerifyAuthorize();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationFeatureToggleIsUnauthorized_ThenShouldReturnErrorFromAuthorizationService()
    {
        _fixture.SetAuthorizationResultAsyncWithEmployerFeatureNotEnabledErrors();

        var result = await _fixture.GetAuthorizationResultAsync();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized == false
                                              && r2.Errors.Count() == 1
                                              && r2.HasError<ProviderFeatureNotEnabled>());
    }

    [Test]
    public void IsAuthorized_WhenFeatureToggleFeatureIsNotEnabled_ThenShouldReturnFalse()
    {
        _fixture.SetAuthorizedOptionsWithEmployerFeatureNotEnabled();

        var result = _fixture.IsAuthorized();

        result.Should().BeFalse();
    }
}

public class AuthorizationServiceWithDefaultHandlerTestsFixture
{
    public string[] Options { get; set; }
    public Mock<IAuthorizationContextProvider> MockAuthorizationContextProvider { get; set; }
    public Mock<IAuthorizationContext> MockAuthorizationContext { get; set; }
    public Mock<IAuthorizationService> MockAuthorizationService { get; set; }
    public IAuthorizationService SutAuthorizationServiceWithDefaultHandler { get; set; }
    public Mock<IDefaultAuthorizationHandler> MockDefaultAuthorizationHandler { get; set; }
    public Mock<AuthorizationServiceWithDefaultHandler> MockAuthorizationServiceWithDefaultHandler { get; set; }

    public AuthorizationServiceWithDefaultHandlerTestsFixture()
    {
        MockAuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
        MockAuthorizationContext = new Mock<IAuthorizationContext>();
        MockDefaultAuthorizationHandler = new Mock<IDefaultAuthorizationHandler>();
        MockAuthorizationService = new Mock<IAuthorizationService>();
        MockAuthorizationServiceWithDefaultHandler = new Mock<AuthorizationServiceWithDefaultHandler>();

        MockAuthorizationContextProvider.Setup(p => p.GetAuthorizationContext()).Returns(MockAuthorizationContext.Object);
        MockAuthorizationService.Setup(a => a.AuthorizeAsync(Options)).Returns(Task.FromResult(true));
        MockAuthorizationService.Setup(a => a.GetAuthorizationResult(Options)).Returns(new AuthorizationResult());
        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync()).ReturnsAsync(new AuthorizationResult());
        MockAuthorizationService.Setup(a => a.IsAuthorized(Options)).Returns(true);
        MockAuthorizationService.Setup(a => a.IsAuthorizedAsync(Options)).Returns(Task.FromResult(true));

        SutAuthorizationServiceWithDefaultHandler = new AuthorizationServiceWithDefaultHandler(
            MockAuthorizationContextProvider.Object,
            MockDefaultAuthorizationHandler.Object,
            MockAuthorizationService.Object);
    }

    public void Authorize()
    {
        SutAuthorizationServiceWithDefaultHandler.Authorize(Options);
    }

    public Task AuthorizeAsync()
    {
        return SutAuthorizationServiceWithDefaultHandler.AuthorizeAsync(Options);
    }

    public bool IsAuthorized()
    {
        return SutAuthorizationServiceWithDefaultHandler.IsAuthorized(Options);
    }

    public AuthorizationResult GetAuthorizationResult()
    {
        return SutAuthorizationServiceWithDefaultHandler.GetAuthorizationResult(Options);
    }

    public Task<AuthorizationResult> GetAuthorizationResultAsync()
    {
        return SutAuthorizationServiceWithDefaultHandler.GetAuthorizationResultAsync(Options);
    }

    public Task<AuthorizationResult> GetDefaultAuthorizationResultAsync()
    {
        var defaultAuthorizationResult = SutAuthorizationServiceWithDefaultHandler.GetAuthorizationResultAsync(Options);

        return defaultAuthorizationResult;
    }

    public Task<bool> IsAuthorizedAsync()
    {
        return SutAuthorizationServiceWithDefaultHandler.IsAuthorizedAsync(Options);
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizedOptions()
    {
        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult());

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult());

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(new[] { "Test" }, MockAuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult());

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetUnauthorizedOptions()
    {
        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult().AddError(new TestTier2UserAccesNotGranted()));

        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult().AddError(new TestEmployerUserRoleNotAuthorized()));

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(new[] { "TestUnAuthorised" }, MockAuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult().AddError(new TestTier2UserAccesNotGranted()));

        return this;
    }


    public AuthorizationServiceWithDefaultHandlerTestsFixture SetUnauthorizedOptionsforDefaultHandler()
    {
        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult());

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult().AddError(new TestTier2UserAccesNotGranted()));

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(new[] { "TestUnAuthorised" }, MockAuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult().AddError(new TestTier2UserAccesNotGranted()));

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizationResult()
    {
        MockAuthorizationService.Setup(x => x.GetAuthorizationResult(Options)).Returns(new AuthorizationResult());

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizationResultAsync()
    {
        MockAuthorizationService.Setup(x => x.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult());

        MockDefaultAuthorizationHandler.Setup(x => x.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult());

        return this;
    }


    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizationResultAsyncWithErrors()
    {
        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult().AddError(new TestTier2UserAccesNotGranted()));

        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult().AddError(new TestEmployerUserRoleNotAuthorized()));

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizedOptionsWithEmployerFeatureNotEnabled()
    {
        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult().AddError(new ProviderFeatureNotEnabled()));

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult());

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(new[] { "Test" }, MockAuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult());

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizationResultAsyncWithEmployerFeatureNotEnabledErrors()
    {
        MockAuthorizationService.Setup(a => a.GetAuthorizationResultAsync(Options)).ReturnsAsync(new AuthorizationResult().AddError(new ProviderFeatureNotEnabled()));

        MockDefaultAuthorizationHandler.Setup(d => d.GetAuthorizationResult(Options, MockAuthorizationContext.Object)).ReturnsAsync(new AuthorizationResult());

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture VerifyAuthorizationService()
    {
        MockAuthorizationService.Verify(x => x.GetAuthorizationResult(Options), Times.Once);

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture VerifyAuthorizationResultAsyncService()
    {
        MockAuthorizationService.Verify(x => x.GetAuthorizationResultAsync(Options), Times.Once);

        MockDefaultAuthorizationHandler.Verify(x => x.GetAuthorizationResult(Options, MockAuthorizationContext.Object), Times.Once);

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizeAsync()
    {
        MockAuthorizationService.Setup(x => x.AuthorizeAsync(Options)).Returns(Task.FromResult(true));

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture SetAuthorizeAsyncReturnsFalse()
    {
        MockAuthorizationService.Setup(x => x.AuthorizeAsync(Options)).Returns(Task.FromResult(false));

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture VerifyAuthorizeAsyncService()
    {
        MockAuthorizationService.Verify(x => x.AuthorizeAsync(Options), Times.Once);

        return this;
    }

    public AuthorizationServiceWithDefaultHandlerTestsFixture VerifyAuthorize()
    {
        MockAuthorizationService.Verify(x => x.Authorize(Options), Times.Once);

        return this;
    }
}

public class TestTier2UserAccesNotGranted : AuthorizationError
{
    public TestTier2UserAccesNotGranted() : base("Tier2 User permission is not granted")
    {
    }
}

public class TestEmployerUserRoleNotAuthorized : AuthorizationError
{
    public TestEmployerUserRoleNotAuthorized() : base("Employer user role is not authorized")
    {
    }
}