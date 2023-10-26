using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class NavigationBarSettingsExtensions
    {
        public static IMvcBuilder AddNavigationBarSettings(this IMvcBuilder builder, IConfiguration configuration)
        {
            builder.SetDefaultNavigationSection(NavigationSection.YourCohorts);
           
            return builder;
        }
    }
}
