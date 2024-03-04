using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderCommitments.Web.Authorization.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization.FeatureToggles;

[TestFixture]
[Parallelizable]
public class FeatureTogglesServiceTests : FluentTest<FeatureTogglesServiceTestsFixture>
{
    [Test]
    public void GetFeatureToggle_WhenFeatureToggleExistsForFeature_ThenShouldReturnFeatureToggle()
    {
        Test(f => f.SetFeatureToggle(), f => f.GetFeatureToggle(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.FeatureToggle));
    }
        
    [Test]
    public void GetFeatureToggle_WhenFeatureConfigurationIsNull_ThenShouldReturnDisabledFeatureToggle()
    {
        Test(f => f.SetFeatureConfigurationToNull().GetFeatureToggle(), (f, r) => r.Should().NotBeNull().And.Match<FeatureToggle>(t => t.Feature == f.Feature && t.IsEnabled == false));
    }

    [Test]
    public void GetFeatureToggle_WhenFeatureToggleDoesNotExistForFeature_ThenShouldReturnDisabledFeatureToggle()
    {
        Test(f => f.GetFeatureToggle(), (f, r) => r.Should().NotBeNull().And.Match<FeatureToggle>(t => t.Feature == f.Feature && t.IsEnabled == false));
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