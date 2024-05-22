using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.Handlers;

[TestFixture]
[Parallelizable]
public class ProviderAuthorizationHandlerTests
{
    private ProviderFeaturesAuthorizationHandlerTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new ProviderFeaturesAuthorizationHandlerTestsFixture();

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreNotAvailable_ThenShouldReturnValidAuthorizationResult()
    {
        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public void GetAuthorizationResult_WhenAndedOptionsAreAvailable_ThenShouldThrowNotImplementedException()
    {
        _fixture.SetAndedOptions();

        var action = async () => await _fixture.GetAuthorizationResult();

        action.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void GetAuthorizationResult_WhenOredOptionIsAvailable_ThenShouldThrowNotImplementedException()
    {
        _fixture.SetOredOption();
        var action = async () => await _fixture.GetAuthorizationResult();
        action.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsEnabledAndWhitelistIsEnabledAndAuthorizationContextIsMissingUkprn_ThenShouldThrowKeyNotFoundException()
    {
        _fixture.SetOption()
            .SetFeatureToggle(true, false, false)
            .SetAuthorizationContextMissingUkprn();

        var action = async () => await _fixture.GetAuthorizationResult();

        action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public void GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsEnabledAndWhitelistIsEnabledAndAuthorizationContextIsMissingUserEmail_ThenShouldThrowKeyNotFoundException()
    {
        _fixture.SetOption()
            .SetFeatureToggle(true, false, false)
            .SetAuthorizationContextMissingUserEmail();

        var action = async () => await _fixture.GetAuthorizationResult();

        action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabled_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(true);

        var authorizationResult = await _fixture.GetAuthorizationResult();

        authorizationResult.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelisted_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(true, true);

        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelistedAndUserEmailIsWhitelisted_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(true, true, true);

        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsNotEnabled_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(false);

        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureNotEnabled>());
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsNotWhitelisted_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(true, false);
        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureUserNotWhitelisted>());
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelistedAndUserEmailIsNotWhitelisted_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        _fixture.SetOption()
            .SetAuthorizationContextValues()
            .SetFeatureToggle(true, true, false);

        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureUserNotWhitelisted>());
    }
}

public class ProviderFeaturesAuthorizationHandlerTestsFixture
{
    public List<string> Options { get; set; }
    public IAuthorizationContext AuthorizationContext { get; set; }
    public IAuthorizationHandler Handler { get; set; }
    public Mock<IFeatureTogglesService<FeatureToggle>> FeatureTogglesService { get; set; }

    public const long Ukprn = 1;
    public const string UserEmail = "foo@bar.com";

    public ProviderFeaturesAuthorizationHandlerTestsFixture()
    {
        Options = new List<string>();
        AuthorizationContext = new AuthorizationContext();
        FeatureTogglesService = new Mock<IFeatureTogglesService<FeatureToggle>>();
        Handler = new ProviderFeaturesAuthorizationHandler(FeatureTogglesService.Object);
    }

    public Task<AuthorizationResult> GetAuthorizationResult()
    {
        return Handler.GetAuthorizationResult(Options, AuthorizationContext);
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetNonProviderFeatureOptions()
    {
        Options.AddRange(new[] { "Foo", "Bar" });

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetAndedOptions()
    {
        Options.AddRange(new[] { "ProviderRelationships", "Tickles" });

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetOredOption()
    {
        Options.Add($"ProviderRelationships,ProviderRelationships");

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetOption()
    {
        Options.AddRange(new[] { "ProviderRelationships" });

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetAuthorizationContextMissingUkprn()
    {
        AuthorizationContext.Set(AuthorizationContextKeys.UserEmail, UserEmail);

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetAuthorizationContextMissingUserEmail()
    {
        AuthorizationContext.Set(AuthorizationContextKeys.Ukprn, Ukprn);

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetAuthorizationContextValues(long ukprn = Ukprn, string userEmail = UserEmail)
    {
        AuthorizationContext.AddProviderFeatureValues(ukprn, userEmail);

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetFeatureToggle(bool isEnabled, bool? isUkprnWhitelisted = null, bool? isUserEmailWhitelisted = null)
    {
        var option = Options.Single();
        var whitelist = new List<ProviderFeatureToggleWhitelistItem>();

        if (isUkprnWhitelisted != null)
        {
            var userEmails = new List<string>();

            if (isUserEmailWhitelisted != null)
            {
                userEmails.Add(isUserEmailWhitelisted == true ? UserEmail : "");
            }

            whitelist.Add(new ProviderFeatureToggleWhitelistItem { Ukprn = isUkprnWhitelisted == true ? Ukprn : 0, UserEmails = userEmails });
        }

        FeatureTogglesService.Setup(s => s.GetFeatureToggle(option)).Returns(new FeatureToggle { Feature = "ProviderRelationships", IsEnabled = isEnabled, Whitelist = whitelist });

        return this;
    }
}