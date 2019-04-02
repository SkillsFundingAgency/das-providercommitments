using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition
{
    public class Features : IFeatures
    {
        private readonly Lazy<FeaturesCache> _lazyFeatures;
        private readonly Lazy<FeatureConfiguration> _featureConfiguration;

        public Features(Lazy<FeatureConfiguration> featureConfiguration)
        {
            _lazyFeatures = new Lazy<FeaturesCache>(InitialiseFeatures);
            _featureConfiguration = featureConfiguration;
        }

        public IEnumerable<Feature> DisabledFeatures => _lazyFeatures.Value.DisabledFeatures;
        public IEnumerable<Feature> EnabledFeatures => _lazyFeatures.Value.EnabledFeatures;

        private class FeaturesCache
        {
            public FeaturesCache(IEnumerable<Feature> allFeatures)
            {
                var allFeaturesTemp = allFeatures.ToArray();
                DisabledFeatures = allFeaturesTemp.Where(f => !f.IsEnabled).ToArray();
                EnabledFeatures = allFeaturesTemp.Where(f => f.IsEnabled).ToArray();
            }

            public Feature[] DisabledFeatures { get; }
            public Feature[] EnabledFeatures { get; }
        }

        private FeaturesCache InitialiseFeatures()
        {
            var featureConfig = _featureConfiguration.Value;

            var features = featureConfig.FeatureDefinitions.Select(fd => new Feature
            {
                EndPoints = fd.Endpoints,
                Name = fd.Name,
                IsEnabled = featureConfig.IsFeatureEnabled(fd.Name)
            });

            var featuresCache = new FeaturesCache(features);

            return featuresCache;
        }
    }
}