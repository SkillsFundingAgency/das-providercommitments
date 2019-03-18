namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    public static class RouteValueKeys
    {
        public static RouteValueKeyPair AccountId = new RouteValueKeyPair("EmployerAccountPublicHashedId", "AccountId");
        public static RouteValueKeyPair AccountLegalEntityId =  new RouteValueKeyPair("EmployerAccountLegalEntityPublicHashedId", "AccountLegalEntityId");
    }
}
