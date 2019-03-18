using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    /// <summary>
    ///     Defines a mapping between the name of a property on the input request (i.e. a route data or query param name)
    ///     to a value that will be placed into the <see cref="IHashingValues"/>.
    /// </summary>
    public class RouteValueKeyPair
    {
        public RouteValueKeyPair(string routeValueKey, string unhashedValueKey)
        {
            RouteValueKey = routeValueKey;
            UnhashedValueKey = unhashedValueKey;
            HashedValueKey = $"Hashed{unhashedValueKey}";
        }

        /// <summary>
        ///     The name of a property that will be looked for in the in-bound request.
        /// </summary>
        public string RouteValueKey { get; }

        /// <summary>
        ///     The name of the property that will be inserted into the <see cref="IHashingValues"/> for the un-hashed value.
        /// </summary>
        public string UnhashedValueKey { get; }

        /// <summary>
        ///     The name of the property that will be inserted into the <see cref="IHashingValues"/> for the original hashed value.
        /// </summary>
        public string HashedValueKey { get; }
    }
}