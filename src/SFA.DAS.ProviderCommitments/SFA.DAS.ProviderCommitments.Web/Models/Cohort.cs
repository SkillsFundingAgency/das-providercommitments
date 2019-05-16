using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class Cohort : IAuthorizationContextModel
    {
        [Required]
        public long? CohortId { get; set; }

        [Required]
        public string CohortPublicHashedId { get; set; }
    }
}
