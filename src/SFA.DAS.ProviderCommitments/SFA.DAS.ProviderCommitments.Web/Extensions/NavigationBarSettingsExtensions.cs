using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class NavigationBarSettingsExtensions
    {
        public static IMvcBuilder AddNavigationBarSettings(this IMvcBuilder builder, IConfiguration configuration)
        {
            builder.SetDefaultNavigationSection(NavigationSection.YourCohorts);
            
            builder.SuppressNavigationSection(NavigationSection.Reservations);

            var featuresConfiguration = configuration
                .GetSection(ProviderCommitmentsConfigurationKeys.FeaturesConfiguration)
                .Get<ProviderFeaturesConfiguration>();

            var manageApprenticesV2Toggle =
                featuresConfiguration.FeatureToggles.SingleOrDefault(x =>
                    x.Feature == ProviderFeature.ManageApprenticesV2WithoutPrefix);

            if (manageApprenticesV2Toggle == null || !manageApprenticesV2Toggle.IsEnabled)
            {
                builder.SuppressNavigationSection(NavigationSection.ManageApprentices);
            }

            return builder;
        }
    }
}
