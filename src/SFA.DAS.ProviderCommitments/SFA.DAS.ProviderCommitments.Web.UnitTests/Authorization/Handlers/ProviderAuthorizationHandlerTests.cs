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
public class ProviderAuthorizationHandlerTests : FluentTest<ProviderFeaturesAuthorizationHandlerTestsFixture>
{
    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreNotAvailable_ThenShouldReturnValidAuthorizationResult()
    {
        return TestAsync(f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }
        
    [Test]
    public Task GetAuthorizationResult_WhenAndedOptionsAreAvailable_ThenShouldThrowNotImplementedException()
    {
        return TestExceptionAsync(f => f.SetAndedOptions(), f => f.GetAuthorizationResult(), (f, r) => r.Should().ThrowAsync<NotImplementedException>());
    }
        
    [Test]
    public Task GetAuthorizationResult_WhenOredOptionIsAvailable_ThenShouldThrowNotImplementedException()
    {
        return TestExceptionAsync(f => f.SetOredOption(), f => f.GetAuthorizationResult(), (f, r) => r.Should().ThrowAsync<NotImplementedException>());
    }
        
    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsEnabledAndWhitelistIsEnabledAndAuthorizationContextIsMissingUkprn_ThenShouldThrowKeyNotFoundException()
    {
        return TestExceptionAsync(
            f => f.SetOption().SetFeatureToggle(true, false, false).SetAuthorizationContextMissingUkprn(), 
            f => f.GetAuthorizationResult(), 
            (f, r) => r.Should().ThrowAsync<KeyNotFoundException>());
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsEnabledAndWhitelistIsEnabledAndAuthorizationContextIsMissingUserEmail_ThenShouldThrowKeyNotFoundException()
    {
        return TestExceptionAsync(
            f => f.SetOption().SetFeatureToggle(true, false, false).SetAuthorizationContextMissingUserEmail(), 
            f => f.GetAuthorizationResult(), 
            (f, r) => r.Should().ThrowAsync<KeyNotFoundException>());
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabled_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(true), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelisted_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(true, true), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelistedAndUserEmailIsWhitelisted_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(true, true, true), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsNotEnabled_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(false), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureNotEnabled>()));
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsNotWhitelisted_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(true, false), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureUserNotWhitelisted>()));
    }

    [Test]
    public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAuthorizationContextIsAvailableAndFeatureIsEnabledAndUkprnIsWhitelistedAndUserEmailIsNotWhitelisted_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        return TestAsync(f => f.SetOption().SetAuthorizationContextValues().SetFeatureToggle(true, true, false), f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull()
            .And.Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureUserNotWhitelisted>()));
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
        Options.AddRange(new [] { "Foo", "Bar" });

        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetAndedOptions()
    {
        Options.AddRange(new [] { "ProviderRelationships", "Tickles" });
            
        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetOredOption()
    {
        Options.Add($"ProviderRelationships,ProviderRelationships");
            
        return this;
    }

    public ProviderFeaturesAuthorizationHandlerTestsFixture SetOption()
    {
        Options.AddRange(new [] { "ProviderRelationships" });

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