using SFA.DAS.ProviderCommitments.Web.Authorization.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.FeatureToggles
{
    public interface IFeatureTogglesService<T> where T : FeatureToggle, new()
    {
        T GetFeatureToggle(string feature);
    }
}