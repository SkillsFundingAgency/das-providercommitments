using System.Collections.Generic;
using SFA.DAS.Authorization.Features.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.Models;

namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class ProviderFeaturesConfiguration : IFeaturesConfiguration<ProviderFeatureToggle>
    {
        public List<ProviderFeatureToggle> FeatureToggles { get; set; }
    }
}