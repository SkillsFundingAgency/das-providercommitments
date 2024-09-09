using System;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Services;
using ProviderPermissionNotGranted = SFA.DAS.ProviderCommitments.Web.Authorization.Errors.ProviderPermissionNotGranted;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.Services;

[TestFixture]
[Parallelizable]
public class AuthorizationServiceTests
{
    private AuthorizationServiceTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new AuthorizationServiceTestsFixture();

    [Test]
    public void Authorize_WhenOperationIsAuthorized_ThenShouldNotThrowException()
    {
        _fixture.SetAuthorizedOptions();
        
        var act = () => _fixture.Authorize();
        
        act.Should().NotThrow();
    }

    [Test]
    public void Authorize_WhenOperationIsUnauthorized_ThenShouldThrowException()
    {
        _fixture.SetUnauthorizedOptions();
        
        var act = () => _fixture.Authorize();
        
        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Test]
    public void AuthorizeAsync_WhenOperationIsAuthorized_ThenShouldNotThrowException()
    {
        _fixture.SetAuthorizedOptions();
        
        var act = async () => await _fixture.AuthorizeAsync();
        
        act.Should().NotThrowAsync();
    }

    [Test]
    public void AuthorizeAsync_WhenOperationIsUnauthorized_ThenShouldThrowException()
    {
        _fixture.SetUnauthorizedOptions();
        
        var act = async () => await _fixture.AuthorizeAsync();
        
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task IsAuthorizedAsync_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        _fixture.SetAuthorizedOptions();
        
        var result = await _fixture.IsAuthorizedAsync();
        
        result.Should().BeTrue();
    }

    [Test]
    public async Task IsAuthorizedAsync_WhenOperationIsUnauthorized_ThenShouldReturnTrue()
    {
        _fixture.SetUnauthorizedOptions();
        
        var result = await _fixture.IsAuthorizedAsync();
        
        result.Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        _fixture.SetAuthorizedOptions();
        
        var result = _fixture.IsAuthorized();
        
        result.Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_WhenOperationIsUnauthorized_ThenShouldReturnTrue()
    {
        _fixture.SetUnauthorizedOptions();
        
        var result = _fixture.IsAuthorized();
        
        result.Should().BeFalse();
    }

    [Test]
    public async Task GetAuthorizationResultAsync_WhenOperationIsAuthorized_ThenShouldReturnValidAuthorizationResult()
    {
        _fixture.SetAuthorizedOptions();
        
        var result = await _fixture.GetAuthorizationResultAsync();
        
        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public void GetAuthorizationResultAsync_WhenOperationIsUnrecognized_ThenShouldThrowException()
    {
        _fixture.SetUnrecognizedOptions();
        
        var action = async () => await _fixture.GetAuthorizationResultAsync();
        
        action.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsAuthorized_ThenShouldReturnValidAuthorizationResult()
    {
        _fixture.SetAuthorizedOptions();

        var result = _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsUnauthorized_ThenShouldReturnInvalidAuthorizationResult()
    {
        _fixture.SetUnauthorizedOptions();

        var result = _fixture.GetAuthorizationResult();

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeFalse();
        result.Errors.Should().HaveCount(1).And.Contain(_fixture.ProviderPermissionNotGranted);
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsUnrecognized_ThenShouldThrowException()
    {
        _fixture.SetUnrecognizedOptions();

        var result = () => _fixture.GetAuthorizationResult();

        result.Should().Throw<ArgumentException>();
    }
}

public class AuthorizationServiceTestsFixture
{
    public string[] Options { get; set; }
    public Mock<IAuthorizationContextProvider> AuthorizationContextProvider { get; set; }
    public Mock<IAuthorizationContext> AuthorizationContext { get; set; }
    public IAuthorizationService AuthorizationService { get; set; }
    public Mock<IAuthorizationHandler> ProviderFeatureAuthorizationHandler { get; set; }
    public ProviderPermissionNotGranted ProviderPermissionNotGranted { get; set; }
    public ProviderFeatureUserNotWhitelisted ProviderFeatureUserNotWhitelisted { get; set; }


    public AuthorizationServiceTestsFixture()
    {
        AuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
        AuthorizationContext = new Mock<IAuthorizationContext>();
        ProviderFeatureAuthorizationHandler = new Mock<IAuthorizationHandler>();
        ProviderFeatureUserNotWhitelisted = new ProviderFeatureUserNotWhitelisted();

        AuthorizationContextProvider.Setup(p => p.GetAuthorizationContext()).Returns(AuthorizationContext.Object);
        ProviderFeatureAuthorizationHandler.Setup(h => h.Prefix).Returns("ProviderFeature.");

        AuthorizationService = new AuthorizationService(AuthorizationContextProvider.Object, new List<IAuthorizationHandler>
        {
            ProviderFeatureAuthorizationHandler.Object,
        });
    }

    public void Authorize()
    {
        AuthorizationService.Authorize(Options);
    }

    public Task AuthorizeAsync()
    {
        return AuthorizationService.AuthorizeAsync(Options);
    }

    public bool IsAuthorized()
    {
        return AuthorizationService.IsAuthorized(Options);
    }

    public Task<bool> IsAuthorizedAsync()
    {
        return AuthorizationService.IsAuthorizedAsync(Options);
    }

    public AuthorizationResult GetAuthorizationResult()
    {
        return AuthorizationService.GetAuthorizationResult(Options);
    }

    public Task<AuthorizationResult> GetAuthorizationResultAsync()
    {
        return AuthorizationService.GetAuthorizationResultAsync(Options);
    }

    public AuthorizationServiceTestsFixture SetAuthorizedOptions()
    {
        Options = new[]
        {
            "ProviderFeature.ProviderRelationships",
        };

        ProviderFeatureAuthorizationHandler.Setup(h => h.GetAuthorizationResult(
                new[] { "ProviderRelationships" }, AuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult());

        return this;
    }

    public AuthorizationServiceTestsFixture SetUnauthorizedOptions()
    {
        Options = new[]
        {
            "ProviderFeature.ProviderRelationships",
        };

        ProviderPermissionNotGranted = new ProviderPermissionNotGranted();

        ProviderFeatureAuthorizationHandler.Setup(h => h.GetAuthorizationResult(
                new[] { "ProviderRelationships" }, AuthorizationContext.Object))
            .ReturnsAsync(new AuthorizationResult(ProviderPermissionNotGranted));

        return this;
    }

    public void SetUnrecognizedOptions()
    {
        Options = new[]
        {
            "Foo",
            "Bar",
            "Foobar"
        };
    }
}