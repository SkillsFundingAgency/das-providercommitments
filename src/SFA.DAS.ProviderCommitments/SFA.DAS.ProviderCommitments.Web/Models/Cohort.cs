using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class Cohort
    {
        [Required]
        public long? CohortId { get; set; }

        [Required]
        public string HashedCohortId { get; set; }
    }
}
