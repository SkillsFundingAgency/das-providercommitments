using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.ModelBinding;
using SFA.DAS.ProviderCommitments.Services.Temp;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    /// <summary>
    ///     Will inspect the current action context for a named input property and if it finds such a property
    ///     it will use the supplied hashing service to un-hash this value and place the resultant un-hashed value
    ///     into the <see cref="IAuthorizationContext"/> using the name specified in <see cref="RouteValueAuthorizationKeyPair"/>.
    /// </summary>
    public abstract class HashedPropertyModelBinder : IHashedPropertyModelBinder
    {
        private readonly IHashingService _hashingService;
        private readonly RouteValueAuthorizationKeyPair _mapping;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="hashingService">
        ///     The hashing service that will be used to un-hash the hashed value if found in the incoming request.
        /// </param>
        protected HashedPropertyModelBinder(IHashingService hashingService, RouteValueAuthorizationKeyPair mapping)
        {
            _hashingService = hashingService;
            _mapping = mapping;
       }

        public void BindModel(ActionContext actionContext, IAuthorizationContext authorizationContext)
        {
            if (TryGetHashedValueFromRouteData(actionContext, out var hashedId) 
                || TryGetHashedValueFromQueryParams(actionContext, out hashedId))
            {
                if (!_hashingService.TryDecodeValue(hashedId, out var id))
                {
                    throw new UnauthorizedAccessException();
                }

                authorizationContext.Set(_mapping.AuthorizationContextValueKey, id);
            }
        }

        private bool TryGetHashedValueFromRouteData(ActionContext actionContext, out string hashedId)
        {
            if(actionContext.RouteData.Values.TryGetValue(_mapping.RouteValueKey, out var hashedIdObject))
            {
                hashedId = hashedIdObject.ToString();
            }
            else
            {
                hashedId = null;
            }

            return hashedId != null;
        }

        private bool TryGetHashedValueFromQueryParams(ActionContext actionContext, out string hashedId)
        {
            if(actionContext.HttpContext.Request.Query.TryGetValue(_mapping.RouteValueKey, out var hashedIdStrings))
            {
                hashedId = hashedIdStrings.ToString();
            }
            else
            {
                hashedId = null;
            }

            return hashedId != null;
        }
    }
}