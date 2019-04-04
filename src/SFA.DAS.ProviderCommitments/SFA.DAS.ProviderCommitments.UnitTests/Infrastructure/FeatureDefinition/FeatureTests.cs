using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.FeatureDefinition
{
    [TestFixture]
    public class FeatureTests
    {
        [TestCase("Reservations", FeatureType.Reservations)]
        [TestCase("reservations", FeatureType.Reservations)]
        [TestCase("RESERVATIONS", FeatureType.Reservations)]
        [TestCase("MadeUpFeature", FeatureType.Unknown)]
        public void SetName_ToSpecificValue_ShouldSetFeatureTypeCorrectly(string featureName, FeatureType expectedFeatureType)
        {
            var feature = new Feature {Name = featureName};

            Assert.AreEqual(expectedFeatureType, feature.FeatureType);
        }
    }
}
