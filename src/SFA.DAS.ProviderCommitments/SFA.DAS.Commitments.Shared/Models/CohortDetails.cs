namespace SFA.DAS.Commitments.Shared.Models
{
    public class CohortDetails
    {
        public long CohortId { get; set; }
        public string HashedCohortId { get; set; }
        public string LegalEntityName { get; set; }
        public bool IsFundedByTransfer { get; set; }
    }
}
