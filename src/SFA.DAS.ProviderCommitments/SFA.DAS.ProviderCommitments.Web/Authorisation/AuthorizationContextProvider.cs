using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.ProviderPermissions;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using static System.String;

namespace SFA.DAS.ProviderCommitments.Web.Authorisation
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncodingService _encodingService;
        private readonly IAuthenticationService _authenticationService;
        private const string CohortIdContextKey = "CohortId";

        public AuthorizationContextProvider(IHttpContextAccessor httpContextAccessor, IEncodingService encodingService, IAuthenticationService authenticationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _encodingService = encodingService;
            _authenticationService = authenticationService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            var accountLegalEntityId = GetAccountLegalEntityId();
            var ukprn = GetUkrpn();
            var cohortId = GetCohortId();

            authorizationContext.AddProviderPermissionValues(accountLegalEntityId, ukprn);
            authorizationContext.Set(CohortIdContextKey, cohortId);

            return authorizationContext;
        }

        private long? GetAccountLegalEntityId()
        {
            if (!TryGetValueFromHttpContext(RouteValueKeys.AccountLegalEntityPublicHashedId, out var accountLegalEntityPublicHashedId))
            {
                return null;
            }
            
            if (!_encodingService.TryDecode(accountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId, out var accountLegalEntityId))
            {
                throw new UnauthorizedAccessException();
            }

            return accountLegalEntityId;
        }

        private long? GetCohortId()
        {
            if (!TryGetValueFromHttpContext(RouteValueKeys.CohortReference, out var cohortReference))
            {
                return null;
            }

            if (!_encodingService.TryDecode(cohortReference, EncodingType.PublicAccountLegalEntityId, out var cohortId))
            {
                throw new UnauthorizedAccessException();
            }

            return cohortId;
        }

        private long? GetUkrpn()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetUserClaimValue(ProviderClaims.Ukprn, out var ukprnClaimValue))
            {
                throw new UnauthorizedAccessException();
            }

            if (!long.TryParse(ukprnClaimValue, out var ukprn))
            {
                throw new UnauthorizedAccessException();
            }

            return ukprn;
        }
        
        private bool TryGetValueFromHttpContext(string key, out string value)
        {
            value = null;
            
            if (_httpContextAccessor.HttpContext.GetRouteData().Values.TryGetValue(key, out var routeValue))
            {
                value = (string)routeValue;
            }
            else if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue(key, out var queryStringValue))
            {
                value = queryStringValue;
            }
            else if (_httpContextAccessor.HttpContext.Request.HasFormContentType)
            {
                if (_httpContextAccessor.HttpContext.Request.Form.TryGetValue(key, out var formValue))
                {
                    value = formValue;
                }
            }

            if(IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return true;
        }
    }
}