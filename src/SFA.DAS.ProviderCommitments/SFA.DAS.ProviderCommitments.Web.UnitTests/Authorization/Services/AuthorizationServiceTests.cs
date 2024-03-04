using System;
using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Services;
using SFA.DAS.Testing;
using ProviderPermissionNotGranted = SFA.DAS.ProviderCommitments.Web.Authorization.ProviderPermissionNotGranted;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.Services;

[TestFixture]
[Parallelizable]
public class AuthorizationServiceTests : FluentTest<AuthorizationServiceTestsFixture>
{
    [Test]
    public void Authorize_WhenOperationIsAuthorized_ThenShouldNotThrowException()
    {
        Test(f => f.SetAuthorizedOptions(), f => f.Authorize(), f => { });
    }

    [Test]
    public void Authorize_WhenOperationIsUnauthorized_ThenShouldThrowException()
    {
        TestException(f => f.SetUnauthorizedOptions(), f => f.Authorize(), (f, r) => r.Should().Throw<UnauthorizedAccessException>());
    }

    [Test]
    public Task AuthorizeAsync_WhenOperationIsAuthorized_ThenShouldNotThrowException()
    {
        return TestAsync(f => f.SetAuthorizedOptions(), f => f.AuthorizeAsync(), f => { });
    }

    [Test]
    public Task AuthorizeAsync_WhenOperationIsUnauthorized_ThenShouldThrowException()
    {
        return TestExceptionAsync(f => f.SetUnauthorizedOptions(), f => f.AuthorizeAsync(), (f, r) => r.Should().ThrowAsync<UnauthorizedAccessException>());
    }

    [Test]
    public Task IsAuthorizedAsync_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        return TestAsync(f => f.SetAuthorizedOptions(), f => f.IsAuthorizedAsync(), (f, r) => r.Should().BeTrue());
    }

    [Test]
    public Task IsAuthorizedAsync_WhenOperationIsUnauthorized_ThenShouldReturnTrue()
    {
        return TestAsync(f => f.SetUnauthorizedOptions(), f => f.IsAuthorizedAsync(), (f, r) => r.Should().BeFalse());
    }

    [Test]
    public void IsAuthorized_WhenOperationIsAuthorized_ThenShouldReturnTrue()
    {
        Test(f => f.SetAuthorizedOptions(), f => f.IsAuthorized(), (f, r) => r.Should().BeTrue());
    }

    [Test]
    public void IsAuthorized_WhenOperationIsUnauthorized_ThenShouldReturnTrue()
    {
        Test(f => f.SetUnauthorizedOptions(), f => f.IsAuthorized(), (f, r) => r.Should().BeFalse());
    }

    [Test]
    public Task GetAuthorizationResultAsync_WhenOperationIsAuthorized_ThenShouldReturnValidAuthorizationResult()
    {
        return TestAsync(f => f.SetAuthorizedOptions(), f => f.GetAuthorizationResultAsync(), (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }

    [Test]
    public Task GetAuthorizationResultAsync_WhenOperationIsUnrecognized_ThenShouldThrowException()
    {
        return TestExceptionAsync(f => f.SetUnrecognizedOptions(), f => f.GetAuthorizationResultAsync(), (f, r) => r.Should().ThrowAsync<ArgumentException>());
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsAuthorized_ThenShouldReturnValidAuthorizationResult()
    {
        Test(f => f.SetAuthorizedOptions(), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsUnauthorized_ThenShouldReturnInvalidAuthorizationResult()
    {
        Test(f => f.SetUnauthorizedOptions(), f => f.GetAuthorizationResult(), (f, r) =>
        {
            r.Should().NotBeNull();
            r.IsAuthorized.Should().BeFalse();
            r.Errors.Should().HaveCount(1).And.Contain(f.ProviderPermissionNotGranted);
        });
    }

    [Test]
    public void GetAuthorizationResult_WhenOperationIsUnrecognized_ThenShouldThrowException()
    {
        TestException(f => f.SetUnrecognizedOptions(), f => f.GetAuthorizationResult(), (f, r) => r.Should().Throw<ArgumentException>());
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