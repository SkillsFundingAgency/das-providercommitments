namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    public static class RouteValueKeys
    {
        public static RouteValueAuthorizationKeyPair AccountId = new RouteValueAuthorizationKeyPair("EmployerAccountPublicHashedId", "AccountId");
        public static RouteValueAuthorizationKeyPair AccountLegalEntityPublicHashedId =  new RouteValueAuthorizationKeyPair("EmployerAccountLegalEntityPublicHashedId", "AccountLegalEntityId");
    }
}
