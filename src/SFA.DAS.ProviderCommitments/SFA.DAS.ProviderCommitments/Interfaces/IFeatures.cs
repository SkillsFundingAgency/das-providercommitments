using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.FeatureDefinition;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    /// <summary>
    ///     Provides access to the features defined in the system.
    /// </summary>
    public interface IFeatures
    {
        /// <summary>
        ///     Provides access to the disabled features in the system.
        /// </summary>
        IEnumerable<Feature> DisabledFeatures { get; }

        /// <summary>
        ///     Provides access to the enabled features in the system.
        /// </summary>
        IEnumerable<Feature> EnabledFeatures { get; }
    }
}
