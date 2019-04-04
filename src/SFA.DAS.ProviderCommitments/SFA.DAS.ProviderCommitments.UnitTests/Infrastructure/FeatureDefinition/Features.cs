using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.FeatureDefinition
{
    [TestFixture]
    public class FeaturesTests
    {
        [Test]
        public void DisabledFeatures_WithNoDisabledFeatures_ShouldReturnEmptyArray()
        {
            var fixtures = new FeaturesTestFixtures();

            var features = fixtures.CreateFeatures();

            Assert.AreEqual(0, features.DisabledFeatures.Count());
        }

        [Test]
        public void DisabledFeatures_WithSingleDisabledFeatures_ShouldReturnSingleFeature()
        {
            string featureName = Guid.NewGuid().ToString();

            var fixtures = new FeaturesTestFixtures().AddDisabledFeature(featureName, "endpoint1");

            var features = fixtures.CreateFeatures();

            Assert.AreEqual(featureName, features.DisabledFeatures.Single().Name);
        }

        [Test]
        public void EnabledFeatures_WithNoEnabledFeatures_ShouldReturnEmptyArray()
        {
            var fixtures = new FeaturesTestFixtures();

            var features = fixtures.CreateFeatures();

            Assert.AreEqual(0, features.EnabledFeatures.Count());
        }

        [Test]
        public void EnabledFeatures_WithSingleEnabledFeatures_ShouldReturnSingleFeature()
        {
            // Arrange
            string featureName = Guid.NewGuid().ToString();
            var fixtures = new FeaturesTestFixtures().AddEnabledFeature(featureName, "endpoint1");

            // Act
            var features = fixtures.CreateFeatures();

            // Assert
            Assert.AreEqual(featureName, features.EnabledFeatures.Single().Name);
        }

        [Test]
        public void Features_WithMultipleEnabledAndDisabledFeatures_ShouldReturnExpectedFeatures()
        {
            // Arrange
            var fixtures = new FeaturesTestFixtures();

            var disabledFeatures = Enumerable.Range(1,10).Select(i =>
            {
                var featureName = Guid.NewGuid().ToString();
                fixtures.AddDisabledFeature(featureName);
                return featureName;
            }).ToArray();

            var enabledFeatures = Enumerable.Range(1, 5).Select(i =>
            {
                var featureName = Guid.NewGuid().ToString();
                fixtures.AddEnabledFeature(featureName);
                return featureName;
            }).ToArray();

            // Act
            var features = fixtures.CreateFeatures();

            // Assert
            Assert.AreEqual(disabledFeatures.Length, features.DisabledFeatures.Count(), "Disabled has wrong number of entries");
            Assert.AreEqual(enabledFeatures.Length, features.EnabledFeatures.Count(), "Enabled has wrong number of entries");

            Assert.IsTrue(features.DisabledFeatures.All(f => disabledFeatures.Contains(f.Name)));
            Assert.IsTrue(features.EnabledFeatures.All(f => enabledFeatures.Contains(f.Name)));
        }
    }

    public class FeaturesTestFixtures
    {
        public FeaturesTestFixtures()
        {
            FeatureConfiguration = new FeatureConfiguration();
        }

        public FeatureConfiguration FeatureConfiguration { get; }
        public FeaturesTestFixtures AddDisabledFeature(string name, params string[] endpoints)
        {
            return AddFeature(name, false, endpoints);
        }

        public FeaturesTestFixtures AddEnabledFeature(string name, params string[] endpoints)
        {
            return AddFeature(name, true, endpoints);
        }

        public FeaturesTestFixtures AddFeature(string name, bool enable, params string[] endpoints)
        {
            FeatureConfiguration.FeatureDefinitions = CopyAndAppend(FeatureConfiguration.FeatureDefinitions,
                new Configuration.FeatureDefinition
                {
                    Endpoints = endpoints,
                    Name = name
                });

            if (enable)
            {
                FeatureConfiguration.EnabledFeatures = CopyAndAppend(FeatureConfiguration.EnabledFeatures, name);
            }

            return this;
        }

        public Features CreateFeatures()
        {
            return new Features(new Lazy<FeatureConfiguration>(FeatureConfiguration));
        }

        private T[] CopyAndAppend<T>(T[] existingArray, T newInstance)
        {
            var requiredSize = existingArray.Length + 1;

            var newArray = new T[requiredSize];

            existingArray.CopyTo(newArray, 0);

            newArray[requiredSize - 1] = newInstance;

            return newArray;
        }
    }
}
