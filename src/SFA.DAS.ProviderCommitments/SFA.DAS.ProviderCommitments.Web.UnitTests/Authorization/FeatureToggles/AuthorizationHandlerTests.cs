using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Authorization.Errors;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.FeatureToggles;

[TestFixture]
[Parallelizable]
public class AuthorizationHandlerTests 
{
    private EmployerFeaturesAuthorizationHandlerTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new EmployerFeaturesAuthorizationHandlerTestsFixture();

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
        
        var result = async () => await _fixture.GetAuthorizationResult();
        
        result.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public void GetAuthorizationResult_WhenOredOptionIsAvailable_ThenShouldThrowNotImplementedException()
    {
        _fixture.SetOredOption();
        
        var result = async () => await _fixture.GetAuthorizationResult();
        
        result.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsEnabled_ThenShouldReturnAuthorizedAuthorizationResult()
    {
        _fixture
            .SetOption()
            .SetFeatureToggle(true);

        var result = await _fixture.GetAuthorizationResult();
        
        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => r2.IsAuthorized);
    }

    [Test]
    public async Task GetAuthorizationResult_WhenOptionsAreAvailableAndFeatureIsNotEnabled_ThenShouldReturnUnauthorizedAuthorizationResult()
    {
        _fixture
            .SetOption()
            .SetFeatureToggle(false);

        var result = await _fixture.GetAuthorizationResult();

        result.Should()
            .NotBeNull()
            .And
            .Match<AuthorizationResult>(r2 => !r2.IsAuthorized && r2.Errors.Count() == 1 && r2.HasError<ProviderFeatureNotEnabled>());
    }
}

public class EmployerFeaturesAuthorizationHandlerTestsFixture
{
    public List<string> Options { get; set; }
    public IAuthorizationContext AuthorizationContext { get; set; }
    public IAuthorizationHandler Handler { get; set; }
    public Mock<IFeatureTogglesService<FeatureToggle>> FeatureTogglesService { get; set; }

    public EmployerFeaturesAuthorizationHandlerTestsFixture()
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

    public EmployerFeaturesAuthorizationHandlerTestsFixture SetNonFeatureOptions()
    {
        Options.AddRange(new[] { "Foo", "Bar" });

        return this;
    }

    public EmployerFeaturesAuthorizationHandlerTestsFixture SetAndedOptions()
    {
        Options.AddRange(new[] { "ProviderRelationships", "Tickles" });

        return this;
    }

    public EmployerFeaturesAuthorizationHandlerTestsFixture SetOredOption()
    {
        Options.Add($"ProviderRelationships,ProviderRelationships");

        return this;
    }

    public EmployerFeaturesAuthorizationHandlerTestsFixture SetOption()
    {
        Options.AddRange(new[] { "ProviderRelationships" });

        return this;
    }

    public EmployerFeaturesAuthorizationHandlerTestsFixture SetFeatureToggle(bool isEnabled, bool? isAccountIdWhitelisted = null, bool? isUserEmailWhitelisted = null)
    {
        var option = Options.Single();

        FeatureTogglesService.Setup(s => s.GetFeatureToggle(option)).Returns(new FeatureToggle { Feature = "ProviderRelationships", IsEnabled = isEnabled });

        return this;
    }
}