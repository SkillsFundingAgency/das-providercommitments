namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    public static class RouteValueKeys
    {
        public static RouteValueAuthorizationKeyPair AccountId = new RouteValueAuthorizationKeyPair("EmployerAccountPublicHashedId", "AccountId");
        public static RouteValueAuthorizationKeyPair AccountLegalEntityId =  new RouteValueAuthorizationKeyPair("EmployerAccountLegalEntityPublicHashedId", "AccountLegalEntityId");
    }
}
