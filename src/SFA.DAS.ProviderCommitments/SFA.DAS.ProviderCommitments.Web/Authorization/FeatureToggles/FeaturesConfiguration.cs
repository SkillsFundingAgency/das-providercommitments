using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles;

public interface IFeaturesConfiguration<T> where T : FeatureToggle, new()
{
    List<T> FeatureToggles { get; }
}
    
public class FeaturesConfiguration : IFeaturesConfiguration<FeatureToggle>
{
    public List<FeatureToggle> FeatureToggles { get; set; }
}