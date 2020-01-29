using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Configuration;

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
