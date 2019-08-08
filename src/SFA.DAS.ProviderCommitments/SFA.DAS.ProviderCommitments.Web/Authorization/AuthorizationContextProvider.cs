using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.CommitmentPermissions.Context;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ProviderFeatures.Context;
using SFA.DAS.Authorization.ProviderPermissions.Context;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncodingService _encodingService;
        private readonly IAuthenticationService _authenticationService;

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
            var cohortId = GetCohortId();
            var draftApprenticeshipId = GetDraftApprenticeshipId();
            var services = GetServices();
            var ukprn = GetUkrpn();
            var userEmail = GetUserEmail();
            
            if (cohortId != null)
            {
                authorizationContext.Set(AuthorizationContextKeys.CohortId, cohortId);
            }

            if (draftApprenticeshipId != null)
            {
                authorizationContext.Set(AuthorizationContextKeys.DraftApprenticeshipId, draftApprenticeshipId);
            }

            if (services != null)
            {
                authorizationContext.Set(AuthorizationContextKeys.Services, services);
            }

            if (ukprn != null && userEmail != null)
            {
                authorizationContext.AddProviderFeatureValues(ukprn.Value, userEmail);
            }
            
            if (accountLegalEntityId != null && ukprn != null)
            {
                authorizationContext.AddProviderPermissionValues(accountLegalEntityId.Value, ukprn.Value);
            }
            
            if (cohortId != null && ukprn != null)
            {
                authorizationContext.AddCommitmentPermissionValues(cohortId.Value, Party.Provider, ukprn.Value);
            }

            return authorizationContext;
        }

        private long? GetAccountLegalEntityId()
        {
            return FindAndDecodeValue(RouteValueKeys.AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);
        }

        private long? GetCohortId()
        {
            if (!TryGetValueFromHttpContext(RouteValueKeys.CohortReference, out var cohortReference))
            {
                return null;
            }

            if (!_encodingService.TryDecode(cohortReference, EncodingType.CohortReference, out var cohortId))
            {
                throw new UnauthorizedAccessException($"Cannot decode cohort reference {cohortReference}");
            }

            return cohortId;
        }

        private long? GetDraftApprenticeshipId()
        {
            return FindAndDecodeValue(RouteValueKeys.DraftApprenticeshipId, EncodingType.ApprenticeshipId);
        }

        private IEnumerable<string> GetServices()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetUserClaimValues(ProviderClaims.Service, out var services))
            {
                throw new UnauthorizedAccessException();
            }

            return services;
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

        private string GetUserEmail()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetUserClaimValue(ProviderClaims.Email, out var userEmail))
            {
                throw new UnauthorizedAccessException();
            }

            return userEmail;
        }

        private long? FindAndDecodeValue(string key, EncodingType encodingType)
        {
            if (!TryGetValueFromHttpContext(key, out var encodedValue))
            {
                return null;
            }

            if (!_encodingService.TryDecode(encodedValue, encodingType, out var value))
            {
                throw new UnauthorizedAccessException();
            }

            return value;
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
            else if (_httpContextAccessor.HttpContext.Request.HasFormContentType && _httpContextAccessor.HttpContext.Request.Form.TryGetValue(key, out var formValue))
            {
                value = formValue;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return true;
        }
    }
}