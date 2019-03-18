using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.ModelBinding.Models
{
    [Unhash]
    public class Account
    {
        [Required]
        public long? AccountId { get; set; }

        public string HashedAccountId { get; set; }
    }
}
