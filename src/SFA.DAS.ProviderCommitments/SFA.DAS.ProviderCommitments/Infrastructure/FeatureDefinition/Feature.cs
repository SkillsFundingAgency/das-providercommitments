using System;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition
{
    public class Feature
    {
        private string _name;

        public FeatureType FeatureType { get; private set; }

        public string Name
        {
            get => _name;
            set => SetFeatureType(value);
        }

        public bool IsEnabled { get; set; }

        public string[] EndPoints { get; set; }

        private void SetFeatureType(string name)
        {
            if (string.Equals(_name, name))
            {
                return;
            }

            if (Enum.TryParse(typeof(FeatureType), name, true, out var featureType))
            {
                FeatureType = (FeatureType) featureType;
            }
            else
            {
                FeatureType = FeatureType.Unknown;
            }

            _name = name;
        }
    }
}