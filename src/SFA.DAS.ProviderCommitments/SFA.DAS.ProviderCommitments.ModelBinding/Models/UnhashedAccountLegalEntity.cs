using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.Models
{
    [Unhash]

    public class UnhashedAccountLegalEntity 
    {
        [Required]
        public long? AccountLegalEntityId { get; set; }
    }
}
