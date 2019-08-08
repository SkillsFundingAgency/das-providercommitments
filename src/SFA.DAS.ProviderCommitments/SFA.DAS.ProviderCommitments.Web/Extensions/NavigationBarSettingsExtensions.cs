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

            var featuresConfiguration = configuration
                .GetSection(ProviderCommitmentsConfigurationKeys.FeaturesConfiguration)
                .Get<ProviderFeaturesConfiguration>();

            var reservationsToggle =
                featuresConfiguration.FeatureToggles.SingleOrDefault(x =>
                    x.Feature == ProviderFeature.ReservationsWithoutPrefix);

            if (reservationsToggle == null || !reservationsToggle.IsEnabled)
            {
                builder.SuppressNavigationSection(NavigationSection.Reservations);
            }

            return builder;
        }
    }
}
