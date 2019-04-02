using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class FeatureDefinition
    {
        public FeatureDefinition()
        {
            Endpoints = new string[0];    
        }

        public string Name { get; set; }
        public string[] Endpoints { get; set; }
    }

    public class FeatureConfiguration
    {
        public FeatureConfiguration()
        {
            EnabledFeatures = new string[0];
            FeatureDefinitions = new FeatureDefinition[0];
        }

        public string[] EnabledFeatures { get; set; }
        public FeatureDefinition[] FeatureDefinitions { get; set; }

        public bool IsFeatureEnabled(string name)
        {
            return EnabledFeatures != null && EnabledFeatures.Contains(name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
