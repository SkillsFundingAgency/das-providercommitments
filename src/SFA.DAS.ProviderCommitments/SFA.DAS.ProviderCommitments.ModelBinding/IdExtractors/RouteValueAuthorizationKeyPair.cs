namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    /// <summary>
    ///     Defines a mapping between the name of a property on the input request (i.e. a route data or query param name)
    ///     to a value that will be placed into the <see cref="IAuthorizationContext"/>.
    /// </summary>
    public class RouteValueAuthorizationKeyPair
    {
        public RouteValueAuthorizationKeyPair(string routeValueKey, string authorizationContextValueKey)
        {
            RouteValueKey = routeValueKey;
            AuthorizationContextValueKey = authorizationContextValueKey;
        }

        public string RouteValueKey { get; }
        public string AuthorizationContextValueKey { get; }
    }
}