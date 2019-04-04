using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.ModelBinding.Models
{
    [Unhash]

    public class AccountLegalEntity 
    {
        [Required]
        public long? AccountLegalEntityId { get; set; }

        public string HashedAccountLegalEntityId { get; set; }
    }
}
