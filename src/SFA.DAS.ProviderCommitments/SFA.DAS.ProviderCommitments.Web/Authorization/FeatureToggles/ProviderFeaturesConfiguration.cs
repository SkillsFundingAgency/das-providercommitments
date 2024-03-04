using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;

public class ProviderFeaturesConfiguration : IFeaturesConfiguration<FeatureToggle>
{
    public List<FeatureToggle> FeatureToggles { get; set; }
}