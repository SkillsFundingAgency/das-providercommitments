﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{

    /// <summary>
    ///     Will inspect the current action context for a named input property and if it finds such a property
    ///     it will use the supplied hashing service to un-hash this value and place the resultant un-hashed value
    ///     into the <see cref="IHashingValues"/> using the name specified in <see cref="RouteValueKeyPair"/>.
    /// </summary>
    public abstract class HashedPropertyModelBinder : IHashedPropertyModelBinder
    {
        private readonly IHashingService _hashingService;
        private readonly RouteValueKeyPair _mapping;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="hashingService">
        ///     The hashing service that will be used to un-hash the hashed value if found in the incoming request.
        /// </param>
        protected HashedPropertyModelBinder(IHashingService hashingService, RouteValueKeyPair mapping)
        {
            _hashingService = hashingService;
            _mapping = mapping;
       }

        public void BindModel(ActionContext actionContext, IHashingValues modelBindingHashValues)
        {
            if (TryGetHashedValueFromRouteData(actionContext, out var hashedId) 
                || TryGetHashedValueFromQueryParams(actionContext, out hashedId)
                || TryGetHashedValueFromForm(actionContext, out hashedId))
            {
                if (!_hashingService.TryDecodeValue(hashedId, out var id))
                {
                    throw new UnauthorizedAccessException();
                }

                modelBindingHashValues.Set(_mapping.UnhashedValueKey, id);
                modelBindingHashValues.Set(_mapping.HashedValueKey, hashedId);
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

        private bool TryGetHashedValueFromForm(ActionContext actionContext, out string hashedId)
        {
            hashedId = null;

            if (actionContext.HttpContext.Request.ContentType == null)
            {
                return false;
            }

            if (actionContext.HttpContext.Request.Form.TryGetValue(_mapping.RouteValueKey, out var hashedIdStrings))
            {
                hashedId = hashedIdStrings.ToString();
            }
            
            return hashedId != null;
        }
    }
}