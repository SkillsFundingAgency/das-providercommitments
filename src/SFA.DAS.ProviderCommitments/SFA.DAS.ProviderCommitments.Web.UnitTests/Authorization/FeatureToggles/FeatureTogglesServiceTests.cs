using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.FeatureToggles;

[TestFixture]
[Parallelizable]
public class FeatureTogglesServiceTests
{
    private FeatureTogglesServiceTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new FeatureTogglesServiceTestsFixture();

    [Test]
    public void GetFeatureToggle_WhenFeatureToggleExistsForFeature_ThenShouldReturnFeatureToggle()
    {
        _fixture.SetFeatureToggle();

        var result = _fixture.GetFeatureToggle();

        result.Should().NotBeNull().And.BeSameAs(_fixture.FeatureToggle);
    }

    [Test]
    public void GetFeatureToggle_WhenFeatureConfigurationIsNull_ThenShouldReturnDisabledFeatureToggle()
    {
        var result = _fixture
            .SetFeatureConfigurationToNull()
            .GetFeatureToggle();

        result.Should()
            .NotBeNull()
            .And
            .Match<FeatureToggle>(t => t.Feature == _fixture.Feature && !t.IsEnabled);
    }

    [Test]
    public void GetFeatureToggle_WhenFeatureToggleDoesNotExistForFeature_ThenShouldReturnDisabledFeatureToggle()
    {
        var result = _fixture.GetFeatureToggle();

        result.Should()
            .NotBeNull()
            .And
            .Match<FeatureToggle>(t => t.Feature == _fixture.Feature && !t.IsEnabled);
    }
}

public class FeatureTogglesServiceTestsFixture
{
    public string Feature { get; set; }
    public IFeatureTogglesService<FeatureToggle> FeatureToggleService { get; set; }
    public FeaturesConfiguration FeaturesConfiguration { get; set; }
    public List<FeatureToggle> FeatureToggles { get; set; }
    public FeatureToggle FeatureToggle { get; set; }

    public FeatureTogglesServiceTestsFixture()
    {
        Feature = "ProviderRelationships";
        FeatureToggles = new List<FeatureToggle>();
        FeaturesConfiguration = new FeaturesConfiguration { FeatureToggles = new List<FeatureToggle>() };
    }

    public FeatureToggle GetFeatureToggle()
    {
        FeatureToggleService = new FeatureTogglesService<FeaturesConfiguration, FeatureToggle>(FeaturesConfiguration);

        return FeatureToggleService.GetFeatureToggle(Feature);
    }

    public FeatureTogglesServiceTestsFixture SetFeatureConfigurationToNull()
    {
        FeaturesConfiguration = null;
        return this;
    }

    public FeatureTogglesServiceTestsFixture SetFeatureToggle()
    {
        FeatureToggle = new FeatureToggle { Feature = Feature, IsEnabled = true };
        FeatureToggles.Add(FeatureToggle);
        FeaturesConfiguration.FeatureToggles.Add(FeatureToggle);

        return this;
    }
}