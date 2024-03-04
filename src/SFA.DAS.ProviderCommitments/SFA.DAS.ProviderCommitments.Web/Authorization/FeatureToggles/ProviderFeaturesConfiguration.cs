using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles
{
    public interface IFeaturesConfiguration<T> where T : FeatureToggle, new()
    {
        List<T> FeatureToggles { get; }
    }
    
    public class ProviderFeaturesConfiguration : IFeaturesConfiguration<ProviderFeatureToggle>
    {
        public List<ProviderFeatureToggle> FeatureToggles { get; set; }
    }
}