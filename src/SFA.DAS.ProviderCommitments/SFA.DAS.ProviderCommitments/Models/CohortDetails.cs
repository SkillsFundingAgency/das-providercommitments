namespace SFA.DAS.ProviderCommitments.Models
{
    public class CohortDetails
    {
        public long CohortId { get; set; }
        public string HashedCohortId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string HashedAccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
    }
}
