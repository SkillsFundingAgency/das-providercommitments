using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.Models
{
    [Unhash]
    public class UnhashedAccount
    {
        [Required]
        public long? AccountId { get; set; }
    }
}
