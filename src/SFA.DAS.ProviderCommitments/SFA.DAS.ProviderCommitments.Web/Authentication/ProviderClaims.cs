﻿namespace SFA.DAS.ProviderCommitments.Web.Authentication;

public static class ProviderClaims
{
    public static readonly string Upn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
    public static readonly string DisplayName = "http://schemas.portal.com/displayname";
    public static readonly string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    public static readonly string Email = "http://schemas.portal.com/mail";
    public static readonly string Ukprn = "http://schemas.portal.com/ukprn";
    public static readonly string Service = "http://schemas.portal.com/service";
    public static string AccessibleCohorts => $"http://das/provider/identity/claims/{nameof(AccessibleCohorts)}";
    public static string AccessibleApprenticeships => $"http://das/provider/identity/claims/{nameof(AccessibleApprenticeships)}";
    public static string OperationPermissions => $"http://das/provider/identity/claims/{nameof(OperationPermissions)}";
}