﻿using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Authorization;

namespace SFA.DAS.ProviderCommitments.UnitTests.Authorization;

[TestFixture]
[Parallelizable]
public class AuthorizationResultTests
{
    [Test]
    public void Constructor_WhenConstructingAnAuthorizationResult_ThenShouldConstructAValidAuthorizationResult()
    {
        var result = new AuthorizationResult();

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WhenConstructingAnAuthorizationResultWithAnError_ThenShouldConstructAnInvalidAuthorizationResult()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult(fixture.ProviderPermissionNotGranted);

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeFalse();
        result.Errors.Should().HaveCount(1).And.Contain(fixture.ProviderPermissionNotGranted);
    }

    [Test]
    public void Constructor_WhenConstructingAnAuthorizationResultWithErrors_ThenShouldConstructAnInvalidAuthorizationResult()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult(fixture.Errors);

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeFalse();
        result.Errors.Should().HaveCount(fixture.Errors.Count).And.Contain(fixture.Errors);
    }

    [Test]
    public void AddError_WhenAddingAnError_ThenShouldInvalidateAuthorizationResult()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult().AddError(fixture.ProviderPermissionNotGranted);

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeFalse();
        result.Errors.Should().HaveCount(1).And.Contain(fixture.ProviderPermissionNotGranted);
    }

    [Test]
    public void AddError_WhenAddingAErrors_ThenShouldInvalidateAuthorizationResult()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult().AddError(fixture.ProviderPermissionNotGranted);

        result.Should().NotBeNull();
        result.IsAuthorized.Should().BeFalse();
        result.Errors.Should().HaveCount(fixture.Errors.Count).And.Contain(fixture.Errors);
    }

    [Test]
    public void HasError_WhenAnErrorOfTypeExists_ThenShouldReturnTrue()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult().AddError(fixture.ProviderPermissionNotGranted);

        result.HasError<ProviderPermissionNotGranted>().Should().BeTrue();
    }

    [Test]
    public void ToString_WhenAuthorized_ThenShouldReturnAuthorizedDescription()
    {
        var result = new AuthorizationResult().ToString();

        result.Should().Be($"IsAuthorized: True, Errors: None");
    }

    [Test]
    public void ToString_WhenUnauthorized_ThenShouldReturnUnauthorizedDescription()
    {
        var fixture = new AuthorizationResultTestsFixture();
        var result = new AuthorizationResult().AddError(fixture.ProviderPermissionNotGranted).AddError(fixture.ProviderPermissionNotGranted).ToString();

        result.Should().Be($"IsAuthorized: False, Errors: {fixture.ProviderPermissionNotGranted.Message}, {fixture.ProviderPermissionNotGranted.Message}");
    }
}

public class AuthorizationResultTestsFixture
{
    public ProviderPermissionNotGranted ProviderPermissionNotGranted { get; set; }
    public List<AuthorizationError> Errors { get; set; }

    public AuthorizationResultTestsFixture()
    {
        ProviderPermissionNotGranted = new ProviderPermissionNotGranted();

        Errors = new List<AuthorizationError>
        {
            ProviderPermissionNotGranted
        };
    }
}