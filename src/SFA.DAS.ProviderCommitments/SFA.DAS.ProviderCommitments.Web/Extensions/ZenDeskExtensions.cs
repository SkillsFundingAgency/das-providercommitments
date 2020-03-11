using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ZenDeskExtensions
    {
        public static IMvcBuilder AddZenDeskSettings(this IMvcBuilder builder, IConfiguration config)
        {
            var zenDeskConfiguration = config.GetSection(ProviderCommitmentsConfigurationKeys.ZenDeskConfiguration).Get<ZenDeskConfiguration>();
            builder.SetZenDeskConfiguration(new Provider.Shared.UI.Models.ZenDeskConfiguration { SectionId = zenDeskConfiguration.SectionId, SnippetKey = zenDeskConfiguration.SnippetKey});

            return builder;
        }
    }
}
